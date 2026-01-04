using System.Collections.Generic;
using UnityEngine;

namespace DataDefinition
{
    [CreateAssetMenu(fileName = "PlayerTurnPoolSO", menuName = "Scriptable Objects/Player Turn Pool")]
    public class PlayerTurnPoolSO : ScriptableObject
    {
        public List<PlayerTurnSO> playerTurns;
    }
}