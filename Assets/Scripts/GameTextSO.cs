using UnityEngine;

[CreateAssetMenu(fileName = "GameTextSO", menuName = "Scriptable Objects/GameText")]
public class GameTextSO : ScriptableObject
{
    public string goodEndText;
    public string badEndText;

    public string maintainTextNPC;
    public string maintainTextPlayer;
    public string withdrawTextNPC;
    public string withdrawTextPlayer;
    public string approachTextNPC;
    public string approachTextPlayer;
}
