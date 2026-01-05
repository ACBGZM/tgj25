using System.Collections.Generic;
using UnityEngine;

namespace DataDefinition
{
    [CreateAssetMenu(fileName = "NPCProfileSO", menuName = "Scriptable Objects/NPC Profile SO")]
    public class NPCProfileSO : ScriptableObject
    {
        [Header("Dialogue Pools")]
        public List<DialoguePoolByLanguage> dialoguePools;

        [Header("Hand Distance Config")]
        public HandDistanceConfigSO handDistanceConfig;

        [Header("Conversation Text")]
        public LanguageTextSO introText;

        [Header("Hand Sprites")]
        public Sprite[] npcHandSprites;

        [Header("Audio")]
        public AudioClip npcBGM;

        public DialoguePoolByLanguage GetPoolByLanguage(Language language)
        {
            return dialoguePools.Find(x => x.language == language);
        }
    }
}
