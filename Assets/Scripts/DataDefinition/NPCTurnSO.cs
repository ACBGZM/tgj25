using System.Collections.Generic;
using UnityEngine;

namespace DataDefinition
{
    [CreateAssetMenu(fileName = "NPCTurnSO", menuName = "Scriptable Objects/NPC Turn SO")]
    public class NPCTurnSO : ScriptableObject
    {
        public int level;
        public string text;
        public List<PlayerChoice> playerChoices;
    }
}
