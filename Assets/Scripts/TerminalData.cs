using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "TerminalData", menuName = "ScriptableObjects/TerminalData", order = 1)]
public class TerminalData : ScriptableObject
{
    // Start is called before the first frame update
    //public List<TextAsset> documents;
    public List<Document> documents;

    public Color activeColor;
    public Color activeBGColor;
    public Color defaultColor;
    public Color defaultBGColor;
    public Color inactiveColor;
    public Color inactiveBGColor;
    [SerializeField]
    public int MaxLineLength = 80;
    [SerializeField]
    private int _maxLength;
    public int maxLength {get {return _maxLength;}}
    public int MaxLineCount = 27;
    [SerializeField]
    public int PaddingH = 1;
    public int PaddingV = 1;
    public string version;
    public string header;
    [NonSerialized]
    public char noBreak = '\u00A0';
    [NonSerialized]
    public string newLine = "\n";

    public TranslationData translationData;

    public void OnValidate()
    {
        _maxLength = MaxLineLength - PaddingH*2;
        if(_maxLength <= 0)
            Debug.LogWarning("Max Length = " + _maxLength.ToString() + " which is <= 0!");
    }
    public class Document
    {
        private Entry entry;
        public Document(Entry entry)
        {
            this.entry = entry;
        }

        public string displayName
        {
            get { return entry.title[TranslationData.currentLanguage];}
        }

        public string value
        {
            get { return entry.text[TranslationData.currentLanguage];}
        }
    }

    public void setLanguage(TranslationData.Language lang)
    {
        TranslationData.currentLanguage = lang;
        updateLanguages();
        updateEntries();
    }
    public void updateLanguages()
    {
        translationData.Reload();
    }
    public void updateEntries()
    {
        //load files
        documents = new List<Document>();
        List <Entry> entries = new List<Entry>(Resources.LoadAll<Entry>("Entries"));
        foreach(var entry in entries)
        {
            documents.Add(new Document(entry));
        }
    }
}
