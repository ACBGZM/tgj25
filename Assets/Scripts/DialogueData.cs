using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerChoice
{
    public string textPrompt;
    public string textLine;

    public string npcResponse;
    public int affectionDelta;
}

[CreateAssetMenu(fileName = "NPCTurnSO", menuName = "Scriptable Objects/NPC Turn SO")]
public class NPCTurnSO : ScriptableObject
{
    public string text;
    public List<PlayerChoice> playerChoices;
}

[CreateAssetMenu(fileName = "PlayerTurnSO", menuName = "Scriptable Objects/Player Turn SO")]
public class PlayerTurnSO : ScriptableObject
{
    public PlayerChoice playerTopic;
}

[CreateAssetMenu(fileName = "NPCTurnPoolSO", menuName = "Scriptable Objects/NPC Turn Pool")]
public class NPCTurnPoolSO : ScriptableObject
{
    public List<NPCTurnSO> npcTurns;
}

[CreateAssetMenu(fileName = "PlayerTurnPoolSO", menuName = "Scriptable Objects/Player Turn Pool")]
public class PlayerTurnPoolSO : ScriptableObject
{
    public List<PlayerTurnSO> playerTurns;
}