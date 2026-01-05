using UnityEngine;

namespace DataDefinition
{
    public enum Language
    {
        CN,
        EN,
    }

    [System.Serializable]
    public class DialoguePoolByLanguage
    {
        public Language language;
        public NPCTurnPoolSO npcTurnPool;
        public PlayerTurnPoolSO playerTurnPool;
    }
}