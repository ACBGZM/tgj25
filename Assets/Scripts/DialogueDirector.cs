using System.Collections.Generic;
using UnityEngine;

public enum DialogueState : uint
{
    NPCOpening,    // npc turn only
    PlayerChoosing,
    NPCResponding,
    PlayerConcluding,   // player turn only
    Resolving,
}

public enum TurnInitiator : uint
{
    NPC,
    Player,
}

public enum ConcludingChoice : uint
{
    Approach,
    Withdraw,
}

public class DialogueDirector : MonoBehaviour
{
    [Header("Pools")]
    public NPCTurnPoolSO npcTurnPool;
    public PlayerTurnPoolSO playerTurnPool;

    [Header("Systems")]
    public DialogueUI ui;
    public AffectionSystem affectionSystem;

    [Header("Config")]
    public int playerTopicOptionCount = 2;

    private NPCTurnSO currentNPCTurn;
    private List<PlayerTurnSO> currentPlayerTurnOptions;
    private PlayerChoice selectedPlayerChoice;

    public DialogueState currentState;
    private TurnInitiator currentTurnInitiator = TurnInitiator.NPC;

    void Start()
    {
        InitData();
        StartNextTurn();
    }

    private void InitData()
    {

    }

    private void StartNextTurn()
    {
        selectedPlayerChoice = null;

        if (currentTurnInitiator == TurnInitiator.NPC)
        {
            StartNPCTurn();
        }
        else
        {
            StartPlayerTurn();
        }
    }

    private void StartNPCTurn()
    {
        currentState = DialogueState.NPCOpening;
        currentNPCTurn = PickRandom(npcTurnPool.npcTurns);

        ui.Clear();
        ui.ShowNPCText(currentNPCTurn.text);    // todo: coroutine

        currentState = DialogueState.PlayerChoosing;
        ui.ShowPlayerChoices(currentNPCTurn.playerChoices, OnNPCAnswerSelected);
    }

    private void OnNPCAnswerSelected(int index)
    {
        selectedPlayerChoice = currentNPCTurn.playerChoices[index];
        EnterResponding();
    }

    private void StartPlayerTurn()
    {
        currentPlayerTurnOptions = PickRandomList(playerTurnPool.playerTurns, playerTopicOptionCount);

        ui.Clear();

        List<PlayerChoice> topics = new List<PlayerChoice>();
        foreach (PlayerTurnSO playerTurn in currentPlayerTurnOptions)
        {
            topics.Add(playerTurn.playerTopic);
        }

        currentState = DialogueState.PlayerChoosing;
        ui.ShowPlayerChoices(topics, OnPlayerTopicSelected);
    }

    private void OnPlayerTopicSelected(int index)
    {
        selectedPlayerChoice = currentPlayerTurnOptions[index].playerTopic;
        EnterResponding();
    }

    private void EnterResponding()
    {
        currentState = DialogueState.NPCResponding;
        ui.ShowNPCText(selectedPlayerChoice.npcResponse);

        if (currentTurnInitiator == TurnInitiator.NPC)
        {
            EnterResolve();
        }
        else
        {
            EnterConcluding();
        }
    }

    private void EnterConcluding()
    {
        currentState = DialogueState.PlayerConcluding;

        ui.ShowConcluding(
            onApproach: () => OnConcludingSelected(ConcludingChoice.Approach),
            onWithdraw: () => OnConcludingSelected(ConcludingChoice.Withdraw)
        );
    }

    private void OnConcludingSelected(ConcludingChoice choice)
    {
        EnterResolve();
    }

    private void EnterResolve()
    {
        currentState = DialogueState.Resolving;

        affectionSystem?.ApplyDelta(selectedPlayerChoice.affectionDelta);   // todo

        currentTurnInitiator =
            currentTurnInitiator == TurnInitiator.NPC ? TurnInitiator.Player : TurnInitiator.NPC;

        StartNextTurn();
    }

    T PickRandom<T>(List<T> list)
    {
        if (list == null || list.Count == 0)
        {
            return default;
        }

        return list[Random.Range(0, list.Count)];
    }

    List<T> PickRandomList<T>(List<T> source, int count)
    {
        List<T> copy = new List<T>(source);
        List<T> result = new List<T>();

        count = Mathf.Min(count, copy.Count);

        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, copy.Count);
            result.Add(copy[index]);
            copy.RemoveAt(index);
        }

        return result;
    }

}
