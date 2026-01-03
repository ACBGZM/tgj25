using UnityEngine;
using UnityEngine.UI;

public class HandDistanceView : MonoBehaviour
{
    [SerializeField] private Sprite[] playerHandSprites;
    private Sprite[] _npcHandSprites;

    [SerializeField] private Image playerHandImage;
    [SerializeField] private Image npcHandImage;

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
}
