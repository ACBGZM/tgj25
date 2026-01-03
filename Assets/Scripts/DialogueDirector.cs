using System;
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
    [Header("NPCs")]
    public List<NPCProfileSO> npcProfiles;
    private NPCProfileSO _currentNPC;
    private int _currentNPCIndex = 0;

    [Header("Systems")]
    public DialogueUI dialogueUI;
    private AffectionSystem _affectionSystem;
    public HandDistanceView handDistanceView;

    [Header("Config")]
    public int playerTopicOptionCount = 2;
    public GameTextSO gameTextSO;

    private NPCTurnSO _currentNPCTurn;
    private List<PlayerTurnSO> _currentPlayerTurnOptions;
    private PlayerChoice _selectedPlayerChoice;
    private ConcludingChoice _selectedConcludingChoice;

    public DialogueState currentState;
    private TurnInitiator _currentTurnInitiator = TurnInitiator.NPC;

    private Coroutine _currentSequence;

    private void PlaySequence(IEnumerator sequence)
    {
        if (_currentSequence != null)
        {
            StopCoroutine(_currentSequence);
        }

        _currentSequence = StartCoroutine(sequence);
    }

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

        PlaySequence(ConversationIntroSequence());
    }

    private IEnumerator ConversationIntroSequence()
    {
        handDistanceView.Clear();
        handDistanceView.ShowCenterText(_currentNPC.introText);
        yield return YieldHelper.WaitForSeconds(3.0f);
        handDistanceView.HideCenterText();

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
        _currentNPCTurn = RandomHelper.PickOne(_currentNPC.npcTurnPool.npcTurns);

        PlaySequence(NPCTurnSequence());
    }

    private IEnumerator NPCTurnSequence()
    {
        dialogueUI.Clear();
        dialogueUI.ShowNPCText(_currentNPCTurn.text);
        yield return YieldHelper.WaitForSeconds(2.0f);

        currentState = DialogueState.PlayerChoosing;
        bool choiceMade = false;
        int selectedIndex = -1;

        dialogueUI.ShowPlayerChoices(_currentNPCTurn.playerChoices, index =>
        {
            choiceMade = true;
            selectedIndex = index;
        });

        yield return YieldHelper.WaitUntil(() => choiceMade);

        _selectedPlayerChoice = _currentNPCTurn.playerChoices[selectedIndex];

        dialogueUI.ShowPlayerText(_selectedPlayerChoice.textLine);
        yield return YieldHelper.WaitForSeconds(2.0f);

        currentState = DialogueState.NPCResponding;
        dialogueUI.ShowNPCText(_selectedPlayerChoice.npcResponse);
        yield return YieldHelper.WaitForSeconds(2.0f);

        EnterResolve();
    }

    private void OnNPCAnswerSelected(int index)
    {
        _selectedPlayerChoice = _currentNPCTurn.playerChoices[index];
        EnterResponding();
    }

    private void StartPlayerTurn()
    {
        _currentPlayerTurnOptions =
            RandomHelper.PickRandomList(_currentNPC.playerTurnPool.playerTurns, playerTopicOptionCount);

        PlaySequence(PlayerTurnSequence());
    }

    private IEnumerator PlayerTurnSequence()
    {
        dialogueUI.Clear();

        List<PlayerChoice> topics = new List<PlayerChoice>();
        foreach (PlayerTurnSO playerTurn in _currentPlayerTurnOptions)
        {
            topics.Add(playerTurn.playerTopic);
        }

        currentState = DialogueState.PlayerChoosing;
        bool topicChoosen = false;
        int selectedIndex = -1;

        dialogueUI.ShowPlayerChoices(topics, index =>
        {
            topicChoosen = true;
            selectedIndex = index;
        });

        yield return YieldHelper.WaitUntil(() => topicChoosen);

        _selectedPlayerChoice = _currentPlayerTurnOptions[selectedIndex].playerTopic;

        dialogueUI.ShowPlayerText(_selectedPlayerChoice.textLine);
        yield return YieldHelper.WaitForSeconds(2.0f);

        currentState = DialogueState.NPCResponding;
        dialogueUI.ShowNPCText(_selectedPlayerChoice.npcResponse);
        yield return YieldHelper.WaitForSeconds(2.0f);

        currentState = DialogueState.PlayerConcluding;
        bool concludingChoosen = false;

        dialogueUI.ShowConcluding(
            onApproach: () =>
            {
                OnConcludingSelected(ConcludingChoice.Approach);
                concludingChoosen = true;
            },
            onWithdraw: () =>
            {
                OnConcludingSelected(ConcludingChoice.Withdraw);
                concludingChoosen = true;
            });

        yield return YieldHelper.WaitUntil(() => concludingChoosen);

        EnterResolve();
    }

    private void OnPlayerTopicSelected(int index)
    {
        _selectedPlayerChoice = _currentPlayerTurnOptions[index].playerTopic;
        EnterResponding();
    }

    private void EnterResponding()
    {
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
        int npcResult = 0;
        int playerResult = 0;

        if (_currentTurnInitiator == TurnInitiator.NPC)
        {
            npcResult = _affectionSystem.ResolveNPCTurn();
        }
        else
        {
            playerResult = _affectionSystem.ResolvePlayerTurn(_selectedConcludingChoice == ConcludingChoice.Approach);
        }

        PlaySequence(ResolveSequence(npcResult, playerResult));
    }

    private IEnumerator ResolveSequence(int npcResult, int playerResult)
    {
        handDistanceView.UpdateView(_affectionSystem.PlayerLevel, _affectionSystem.NPCLevel);
        yield return YieldHelper.WaitForSeconds(1.0f);

        if (npcResult == 0 && playerResult == 0)
        {
            handDistanceView.ShowCornerText(gameTextSO.maintainText);
        }
        else if (npcResult == 1 || playerResult == 1)
        {
            handDistanceView.ShowCornerText(gameTextSO.approachText);
        }
        else if (npcResult == -1 || playerResult == -1)
        {
            handDistanceView.ShowCornerText(gameTextSO.withdrawText);
        }

        yield return new WaitForSeconds(2f);
        handDistanceView.HideCornerText();
        dialogueUI.ClearChat();

        ConversationResult result = _affectionSystem.CheckConversationEnded();
        if (result != ConversationResult.None)
        {
            PlaySequence(ConversationEndSequence(result));
            yield break;
        }

        _currentTurnInitiator =
            _currentTurnInitiator == TurnInitiator.NPC ? TurnInitiator.Player : TurnInitiator.NPC;

        StartNextTurn();
    }

    private IEnumerator ConversationEndSequence(ConversationResult result)
    {
        yield return new WaitForSeconds(1.0f);

        handDistanceView.ShowCenterText(result == ConversationResult.Good ? gameTextSO.goodEndText : gameTextSO.badEndText);

        if (_currentNPCIndex >= npcProfiles.Count)
        {
            // todo: game over
            yield break;
        }

        dialogueUI.ShowContinueButton(() =>
        {
            _currentNPCIndex++;
            StartConversation();
        });
    }
}