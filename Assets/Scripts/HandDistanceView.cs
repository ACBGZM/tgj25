using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HandDistanceView : MonoBehaviour
{
    [SerializeField] private Sprite[] playerHandSprites;
    private Sprite[] _npcHandSprites;

    [SerializeField] private Image playerHandImage;
    [SerializeField] private Image npcHandImage;

    [SerializeField] private TextMeshProUGUI centerText;
    [SerializeField] private TextMeshProUGUI cornerText;

    public void Init(Sprite[] npcHandSprites, int playerLevel, int npcLevel)
    {
        _npcHandSprites =  npcHandSprites;
        UpdateView(playerLevel, npcLevel);
    }

    public void UpdateView(int playerLevel, int npcLevel)
    {
        playerHandImage.sprite = playerHandSprites[playerLevel - 1];
        npcHandImage.sprite = _npcHandSprites[npcLevel - 1];
    }

    public void Clear()
    {
        HideCenterText();
        HideCornerText();
    }

    public void ShowCenterText(string text)
    {
        centerText.gameObject.SetActive(true);
        centerText.text = text;
    }

    public void HideCenterText()
    {
        centerText.gameObject.SetActive(false);
    }

    public void ShowCornerText(string text)
    {
        cornerText.gameObject.SetActive(true);
        cornerText.text = text;
    }

    public void HideCornerText()
    {
        cornerText.gameObject.SetActive(false);
    }
}
