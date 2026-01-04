using System.Collections;
using System.Collections.Generic;
using DataDefinition;
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
    public GameOpening gameOpening;

    [Header("Config")]
    public int playerTopicOptionCount = 2;
    public GameTextSO gameTextSO;

    [Header("Audio")]
    [SerializeField] private AudioClip approachAudioClip;
    [SerializeField] private AudioClip withdrawAudioClip;

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
        // StartConversation();
    }

    private void InitData()
    {
        _affectionSystem = new AffectionSystem();
    }

    public void StartConversation()
    {
        _currentNPC = npcProfiles[_currentNPCIndex];

        _affectionSystem.Init(_currentNPC.handDistanceConfig);
        handDistanceView.Init(_currentNPC.npcHandSprites, _affectionSystem.PlayerLevel, _affectionSystem.NPCLevel);

        AudioManager.Instance.PlayNPCBGM(_currentNPC.npcBGM);

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

        int npcLevelFormer = _affectionSystem.NPCLevel;
        int playerLevelFormer = _affectionSystem.PlayerLevel;

        if (_currentTurnInitiator == TurnInitiator.NPC)
        {
            npcResult = _affectionSystem.ResolveNPCTurn();
        }
        else
        {
            playerResult = _affectionSystem.ResolvePlayerTurn(_selectedConcludingChoice == ConcludingChoice.Approach);
        }

        int npcLevelNew =  _affectionSystem.NPCLevel;
        int playerLevelNew =  _affectionSystem.PlayerLevel;

        PlaySequence(ResolveSequence(npcLevelNew - npcLevelFormer, playerLevelNew - playerLevelFormer));
    }

    private IEnumerator ResolveSequence(int npcResult, int playerResult)
    {
        handDistanceView.UpdateView(_affectionSystem.PlayerLevel, _affectionSystem.NPCLevel);
        bool withdraw = npcResult  < 0 || playerResult < 0;
        bool apporach = npcResult > 0 || playerResult > 0;
        bool maintain = npcResult == 0 || playerResult == 0;

        if (withdraw)
        {
            handDistanceView.ShowCornerText(gameTextSO.withdrawText);
            AudioManager.Instance.PlaySFX(withdrawAudioClip);
        }
        else if (apporach)
        {
            handDistanceView.ShowCornerText(gameTextSO.approachText);
            AudioManager.Instance.PlaySFX(approachAudioClip);
        }
        else if (maintain)
        {
            handDistanceView.ShowCornerText(gameTextSO.maintainText);
        }

        yield return YieldHelper.WaitForSeconds(2.0f);

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

        ++_currentNPCIndex;
        if (_currentNPCIndex >= npcProfiles.Count)
        {
            dialogueUI.ShowContinueButton(() =>
            {
                AudioManager.Instance.StopNPCBGM();
                AudioManager.Instance.PlayButtonClickClip();
                ResetGame();
            });
            yield break;
        }

        dialogueUI.ShowContinueButton(() =>
        {
            AudioManager.Instance.StopNPCBGM();
            AudioManager.Instance.PlayButtonClickClip();
            StartConversation();
        });
    }

    private void ResetGame()
    {
        _currentNPCIndex = 0;
        handDistanceView.HideCenterText();

        gameOpening.ResetGame();
    }
}