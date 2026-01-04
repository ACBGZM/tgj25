using UnityEngine;

namespace DataDefinition
{
    [CreateAssetMenu(fileName = "PlayerTurnSO", menuName = "Scriptable Objects/Player Turn SO")]
    public class PlayerTurnSO : ScriptableObject
    {
        public int level;
        public PlayerChoice playerTopic;
    }
}
