using DataDefinition;
using UnityEngine;
using DG.Tweening;

public class GameOpening : MonoBehaviour
{
    [Header("UI")] public CanvasGroup startPanel;
    public CanvasGroup playerView;
    public CanvasGroup npcView;

    [Header("Systems")] public DialogueDirector dialogueDirector;

    [Header("Audio")] [SerializeField] private AudioClip approachAudioClip;

    public void OnClickStart()
    {
        PlayOpening();
    }

    private void PlayOpening()
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(startPanel.DOFade(0, 1.0f))
            .OnComplete(() => startPanel.gameObject.SetActive(false));

        seq.AppendInterval(0.5f);
        seq.AppendCallback(() => AudioManager.Instance.PlaySFX(approachAudioClip));
        seq.Append(playerView.DOFade(1, 1.0f));

        seq.AppendInterval(0.5f);
        seq.AppendCallback(() => AudioManager.Instance.PlaySFX(approachAudioClip));
        seq.Append(npcView.DOFade(1, 1.0f));

        seq.OnComplete(() => { dialogueDirector.StartConversation(); });
    }

    public void ResetGame()
    {
        startPanel.alpha = 1;

        playerView.DOFade(0, 1.0f);
        npcView.DOFade(0, 1.0f);
    }

    public void OnClickSwitchLanguage()
    {
        var lm = LanguageManager.Instance;
        lm.SwitchLanguage(
            lm.CurrentLanguage == Language.CN ? Language.EN : Language.CN
        );
    }
}