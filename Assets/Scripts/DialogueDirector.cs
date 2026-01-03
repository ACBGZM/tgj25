using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DialogueState : uint
{
    NPCOpening, // npc turn only
    PlayerChoosing,
    NPCResponding,
    PlayerConcluding, // player turn only
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
    [Header("NPCs")] public List<NPCProfileSO> npcProfiles;
    private NPCProfileSO _currentNPC;
    private int _currentNPCIndex = 0;

    [Header("Systems")] public DialogueUI dialogueUI;
    private AffectionSystem _affectionSystem;
    public HandDistanceView handDistanceView;

    [Header("Config")] public int playerTopicOptionCount = 2;

    private NPCTurnSO _currentNPCTurn;
    private List<PlayerTurnSO> _currentPlayerTurnOptions;
    private PlayerChoice _selectedPlayerChoice;
    private ConcludingChoice _selectedConcludingChoice;

    public DialogueState currentState;
    private TurnInitiator _currentTurnInitiator = TurnInitiator.NPC;

    void Start()
    {
        InitData();
        StartConversation();
    }

    private void InitData()
    {
        _affectionSystem = new AffectionSystem();
    }

    private void StartConversation()
    {
        _currentNPC = npcProfiles[_currentNPCIndex];

        _affectionSystem.Init(_currentNPC.handDistanceConfig);
        handDistanceView.Init(_currentNPC.npcHandSprites, _affectionSystem.PlayerLevel, _affectionSystem.NPCLevel);

        StartNextTurn();
    }

    private void StartNextTurn()
    {
        _selectedPlayerChoice = null;

        if (_currentTurnInitiator == TurnInitiator.NPC)
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
        _currentNPCTurn = PickRandom(_currentNPC.npcTurnPool.npcTurns);

        dialogueUI.Clear();
        dialogueUI.ShowNPCText(_currentNPCTurn.text); // todo: coroutine

        currentState = DialogueState.PlayerChoosing;
        dialogueUI.ShowPlayerChoices(_currentNPCTurn.playerChoices, OnNPCAnswerSelected);
    }

    private void OnNPCAnswerSelected(int index)
    {
        _selectedPlayerChoice = _currentNPCTurn.playerChoices[index];
        EnterResponding();
    }

    private void StartPlayerTurn()
    {
        _currentPlayerTurnOptions = PickRandomList(_currentNPC.playerTurnPool.playerTurns, playerTopicOptionCount);

        dialogueUI.Clear();

        List<PlayerChoice> topics = new List<PlayerChoice>();
        foreach (PlayerTurnSO playerTurn in _currentPlayerTurnOptions)
        {
            topics.Add(playerTurn.playerTopic);
        }

        currentState = DialogueState.PlayerChoosing;
        dialogueUI.ShowPlayerChoices(topics, OnPlayerTopicSelected);
    }

    private void OnPlayerTopicSelected(int index)
    {
        _selectedPlayerChoice = _currentPlayerTurnOptions[index].playerTopic;
        EnterResponding();
    }

    private void EnterResponding()
    {
        currentState = DialogueState.NPCResponding;
        dialogueUI.ShowNPCText(_selectedPlayerChoice.npcResponse);

        if (_currentTurnInitiator == TurnInitiator.NPC)
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

        dialogueUI.ShowConcluding(
            onApproach: () => OnConcludingSelected(ConcludingChoice.Approach),
            onWithdraw: () => OnConcludingSelected(ConcludingChoice.Withdraw)
        );
    }

    private void OnConcludingSelected(ConcludingChoice choice)
    {
        _selectedConcludingChoice = choice;
        EnterResolve();
    }

    private void EnterResolve()
    {
        currentState = DialogueState.Resolving;

        if (_currentTurnInitiator == TurnInitiator.NPC)
        {
            _affectionSystem.ResolveNPCTurn();
        }
        else
        {
            _affectionSystem.ResolvePlayerTurn(_selectedConcludingChoice == ConcludingChoice.Approach);
        }

        handDistanceView.UpdateView(_affectionSystem.PlayerLevel, _affectionSystem.NPCLevel);

        ConversationResult result = _affectionSystem.CheckConversationEnded();

        if (result == ConversationResult.Good)
        {
            // todo: good end
            StartCoroutine(ProceedToNextNPC());
        }
        else if (result == ConversationResult.Bad)
        {
            // todo: bad end
            StartCoroutine(ProceedToNextNPC());
        }
        else
        {
            _currentTurnInitiator =
                _currentTurnInitiator == TurnInitiator.NPC ? TurnInitiator.Player : TurnInitiator.NPC;

            StartNextTurn();
        }
    }

    IEnumerator ProceedToNextNPC()
    {
        yield return new WaitForSeconds(1.0f);

        _currentNPCIndex++;

        if (_currentNPCIndex >= npcProfiles.Count)
        {
            // todo: game over
            yield break;
        }

        StartConversation();
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