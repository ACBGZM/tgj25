using System.Collections.Generic;
using UnityEngine;

namespace DataDefinition
{
    [CreateAssetMenu(fileName = "NPCTurnPoolSO", menuName = "Scriptable Objects/NPC Turn Pool")]
    public class NPCTurnPoolSO : ScriptableObject
    {
        public List<NPCTurnSO> npcTurns;
    }
}
