using System;
using System.Collections.Generic;
using DataDefinition;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    public Transform chatContentRoot;
    public Transform choicePanelRoot;

    public GameObject npcBubblePrefab;
    public GameObject playerBubblePrefab;
    public GameObject choiceButtonPrefab;

    public ScrollRect chatScrollRect;
    public Transform chatScrollRectContent;

    public GameObject conclusionPanel;
    public Button approachButton;
    public Button withdrawButton;

    [SerializeField] private Button continueButton;

    public void ShowNPCText(string text)
    {
        CreateChatBubble(npcBubblePrefab, text);
        ScrollToBottom();
    }

    public void ShowPlayerText(string text)
    {
        CreateChatBubble(playerBubblePrefab, text);
        ScrollToBottom();
    }

    public void ShowPlayerChoices(List<PlayerChoice> choices, Action<int> onSelect)
    {
        ClearChoices();

        for (int i = 0; i < choices.Count; ++i)
        {
            int index = i;
            var choice = choices[index];

            GameObject choiceButton = Instantiate(choiceButtonPrefab, choicePanelRoot);
            choiceButton.GetComponentInChildren<TextMeshProUGUI>().text = choice.textPrompt;

            Button button = choiceButton.GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                ClearChoices();
                onSelect?.Invoke(index);
            });
        }
    }

    public void Clear()
    {
        ClearChoices();
    }

    private void ClearChoices()
    {
        for (int i = choicePanelRoot.childCount - 1; i >= 0; i--)
        {
            Destroy(choicePanelRoot.GetChild(i).gameObject);
        }
    }

    private void CreateChatBubble(GameObject prefab, string text)
    {
        GameObject go = Instantiate(prefab, chatContentRoot);
        go.GetComponentInChildren<TextMeshProUGUI>().text = text;
    }

    private void ScrollToBottom()
    {
        Canvas.ForceUpdateCanvases();
        chatScrollRect.verticalNormalizedPosition = 0f;
    }

    public void ShowConcluding(Action onApproach, Action onWithdraw)
    {
        conclusionPanel.SetActive(true);

        approachButton.onClick.RemoveAllListeners();
        withdrawButton.onClick.RemoveAllListeners();

        approachButton.onClick.AddListener(() =>
        {
            onApproach?.Invoke();
            conclusionPanel.SetActive(false);
        });
        withdrawButton.onClick.AddListener(() =>
        {
            onWithdraw?.Invoke();
            conclusionPanel.SetActive(false);
        });
    }

    public void ShowContinueButton(Action onSelect)
    {
        continueButton.gameObject.SetActive(true);

        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(() =>
        {
            onSelect?.Invoke();
            continueButton.gameObject.SetActive(false);
        });
    }

    public void ClearChat()
    {
        for (int i = chatScrollRectContent.childCount - 1; i >= 0; i--)
        {
            Destroy(chatScrollRectContent.GetChild(i).gameObject);
        }

        Canvas.ForceUpdateCanvases();
        chatScrollRect.verticalNormalizedPosition = 1f;
    }
}
