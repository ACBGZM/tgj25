using UnityEngine;

namespace DataDefinition
{
    [CreateAssetMenu(fileName = "NPCProfileSO", menuName = "Scriptable Objects/NPC Profile SO")]
    public class NPCProfileSO : ScriptableObject
    {
        [Header("Dialogue Pools")]
        public NPCTurnPoolSO npcTurnPool;
        public PlayerTurnPoolSO playerTurnPool;

        [Header("Hand Distance Config")]
        public HandDistanceConfigSO handDistanceConfig;

        [Header("Conversation Text")]
        public string introText;

        [Header("Hand Sprites")]
        public Sprite[] npcHandSprites;

        [Header("Audio")]
        public AudioClip npcBGM;
    }
}
