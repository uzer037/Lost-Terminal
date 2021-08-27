using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(Entry))]
public class EntryEditor : Editor
{
    private int lang = 0;
    private int lang2 = 1;
    List<string> langs;
    void Awake()
    {
        Init();
    }

    void Init()
    {
        langs = new List<string>();
        foreach(var key in TranslationData.languageNames.Keys)
        {
            langs.Add(TranslationData.languageNames[key]);
        }
    }
    public override void OnInspectorGUI()
    {
        Entry entry = target as Entry;
        int newLang;
        int newLang2;
        if(entry != null)
        {
            GUILayout.Label("UID: " + entry.UID.ToString());
            GUILayout.Space(16);

            if(entry.title == null)
                entry.title = new TranslatedText();
            
            if(entry.text == null)
                entry.text =  new TranslatedText();

            TranslationData.Language lng;
            TranslationData.Language lng2;

            GUILayout.BeginHorizontal();


            GUILayout.BeginVertical();
            newLang = GUILayout.Toolbar(lang, langs.ToArray());

            if(newLang != lang)
            {
                lang = newLang;
            }
            lng = (TranslationData.Language)lang;
            if(!entry.title.ContainsKey(lng))
                entry.title.Add(lng, "");
            if(!entry.text.ContainsKey(lng))
                entry.text.Add(lng, "");

            GUILayout.Label ("Default Language: ");
            GUILayout.BeginHorizontal();
            GUILayout.Label ("Title: ");
            entry.title[lng] = GUILayout.TextField(entry.title[lng]);
            GUILayout.EndHorizontal();
            entry.text[lng] = GUILayout.TextArea(entry.text[lng]);
            GUILayout.EndVertical();


            GUILayout.BeginVertical();
            newLang2 = GUILayout.Toolbar(lang2, langs.ToArray());

            if(newLang2 != lang2)
            {
                lang2 = newLang2;
            }
            lng2 = (TranslationData.Language)lang2;
            if(!entry.title.ContainsKey(lng2))
                entry.title.Add(lng2, "");
            if(!entry.text.ContainsKey(lng2))
                entry.text.Add(lng2, "");

            GUILayout.Label ("Selected Language: ");
            GUILayout.BeginHorizontal();
            GUILayout.Label ("Title: ");
            entry.title[lng2] = GUILayout.TextField(entry.title[lng2]);
            GUILayout.EndHorizontal();
            entry.text[lng2] = GUILayout.TextArea(entry.text[lng2]);
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

        }
        EditorUtility.SetDirty(entry);
        this.Repaint();
    }
}
