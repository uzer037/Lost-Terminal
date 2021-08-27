using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TranslationData", menuName = "ScriptableObjects/Translation/TranslationData", order = 2)]
public class TranslationData : ScriptableObject
{
    [System.Serializable]
    public enum Language { English, Russian }
    public static Dictionary<Language, string> languageNames = new Dictionary<Language, string> {
        { Language.English, "en" },
        { Language.Russian, "ru" }
    };
    public List<Translation> translations;
    //private Dictionary<Language, Translation> _translations;
    public Translation translation
    {
        get
        {
            Translation res = null;
            if (translations != null && translations.Count > 0)
            {
                res = translations[1];
                foreach (var trans in translations)
                {
                    if (trans.language == currentLanguage)
                        res = trans;
                }
            }
            else
            {
                if(translations != null)
                    Debug.Log("Translation is null somehow... How surprising.");
                else
                    Debug.Log("Translations LIST is null somehow... How surprising.");
            }
            if (res == null)
                Debug.LogError("Translation is somehow null!");
            return res;
        }
    }
    //public TranslationData.Language defaultLanguage = Language.English;
    public static TranslationData.Language fallbackLanguage = Language.English;
    public static TranslationData.Language currentLanguage;
    public void Reload()
    {
        Debug.Log("Gdmonin'");
        //_translations = new Dictionary<Language, Translation>();
    }
}
