using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TranslationData", menuName = "ScriptableObjects/Translation/TranslationData", order = 2)]
public class TranslationData : ScriptableObject
{
    [System.Serializable]
    public enum Language { Russian, English }
    public static Dictionary<Language, string> languageNames = new Dictionary<Language, string> {
        { Language.Russian, "ru" },
        { Language.English, "en" }
    };
    public List<Translation> translations;
    private Dictionary<Language, Translation> _translations;
    public Translation translation
    {
        get
        {
            Translation res = null;
            foreach (var trans in translations)
            {
                if (trans.language == currentLanguage)
                    res = trans;
            }
            return res;
        }
    }
    //public TranslationData.Language defaultLanguage = Language.English;
    public static TranslationData.Language fallbackLanguage = Language.English;
    public static TranslationData.Language currentLanguage;
    public void Reload()
    {
        Debug.Log("Gdmonin'");
        _translations = new Dictionary<Language, Translation>();
    }
}
