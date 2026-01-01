using System;
using System.Collections.Generic;
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

    public ScrollRect scrollRect;

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
                ShowPlayerText(choice.textLine);
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
        scrollRect.verticalNormalizedPosition = 0f;
    }
}
