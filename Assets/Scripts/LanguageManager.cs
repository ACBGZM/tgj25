using System.Collections.Generic;
using DataDefinition;
using UnityEngine;

public class LanguageManager : MonoBehaviour
{
    public static LanguageManager Instance;

    public Language CurrentLanguage { get; private set; } = Language.EN;

    private List<LocalizedText> listeners = new();

    void Awake()
    {
        Instance = this;
    }

    public void Register(LocalizedText text)
    {
        listeners.Add(text);
    }

    public void SwitchLanguage(Language language)
    {
        CurrentLanguage = language;
        foreach (var t in listeners)
            t.Refresh();
    }

    public string GetLanguageText(LanguageTextSO languageText)
    {
        return CurrentLanguage == Language.CN ? languageText.chinese : languageText.english;
    }
}