using UnityEngine;

namespace DataDefinition
{
    [CreateAssetMenu(menuName = "LanguageTextSO")]
    public class LanguageTextSO : ScriptableObject
    {
        [TextArea] public string chinese;
        [TextArea] public string english;
    }
}