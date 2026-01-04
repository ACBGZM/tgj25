using UnityEngine;

[CreateAssetMenu(fileName = "GameTextSO", menuName = "Scriptable Objects/GameText")]
public class GameTextSO : ScriptableObject
{
    public string goodEndText;
    public string badEndText;

    public string maintainText;
    public string withdrawText;
    public string approachText;
}
