using DataDefinition;
using UnityEngine;

[CreateAssetMenu(fileName = "GameTextSO", menuName = "Scriptable Objects/GameText")]
public class GameTextSO : ScriptableObject
{
    public LanguageTextSO goodEndText;
    public LanguageTextSO badEndText;

    public LanguageTextSO maintainTextNPC;
    public LanguageTextSO maintainTextPlayer;
    public LanguageTextSO withdrawTextNPC;
    public LanguageTextSO withdrawTextPlayer;
    public LanguageTextSO approachTextNPC;
    public LanguageTextSO approachTextPlayer;
}
