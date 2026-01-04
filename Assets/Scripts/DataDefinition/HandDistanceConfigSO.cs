using UnityEngine;

namespace DataDefinition
{
    [System.Serializable]
    public class DistanceProbability
    {
        [Range(0, 1f)] public float approach;
        [Range(0, 1f)] public float withdraw;
    }

    [CreateAssetMenu(fileName = "HandDistanceConfigSO", menuName = "Scriptable Objects/Hand Distance Config")]
    public class HandDistanceConfigSO : ScriptableObject
    {
        public DistanceProbability level1;
        public DistanceProbability level2;
        public DistanceProbability level3;
        public DistanceProbability level4;
        public DistanceProbability level5;

        public DistanceProbability GetProbability(int level)
        {
            return level switch
            {
                1 => level1,
                2 => level2,
                3 => level3,
                4 => level4,
                5 => level5,
                _ => level3
            };
        }
    }
}