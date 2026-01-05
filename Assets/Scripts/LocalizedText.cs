using DataDefinition;
using TMPro;
using UnityEngine;

public class LocalizedText : MonoBehaviour
{
    public LanguageTextSO textSO;
    private TMP_Text _text;

    void Awake()
    {
        _text = GetComponent<TMP_Text>();
    }

    void Start()
    {
        LanguageManager.Instance.Register(this);
        Refresh();
    }

    public void Refresh()
    {
        _text.text = LanguageManager.Instance.CurrentLanguage == Language.CN ? textSO.chinese : textSO.english;
    }
}