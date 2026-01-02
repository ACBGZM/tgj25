using UnityEngine;
using UnityEngine.UI;

public class HandDistanceView : MonoBehaviour
{
    [SerializeField] private Sprite[] playerHandSprites;
    [SerializeField] private Sprite[] npcHandSprites;

    [SerializeField] private Image playerHandImage;
    [SerializeField] private Image npcHandImage;

    public void Init(int npcIndex, int playerLevel, int npcLevel)
    {
        // todo: multiple npc

        UpdateView(playerLevel, npcLevel);
    }

    public void UpdateView(int playerLevel, int npcLevel)
    {
        playerHandImage.sprite = playerHandSprites[playerLevel - 1];
        npcHandImage.sprite = npcHandSprites[npcLevel - 1];
    }
}
