using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Translation", menuName = "ScriptableObjects/Translation/Translation", order = 2)]
[System.Serializable]
public class Translation : ScriptableObject
{
    //TranslationFields
    public int translationIndex;
    public TranslationData.Language language;
    public EditorData editorData;

    [System.Serializable]
    public struct EditorData
    {
        public string languageFullName;
    }
    public UI ui;
    [System.Serializable]
    public struct UI
    {
        public string button_back;
        public string button_ok;
        public string button_cancel;
        public Main main;

        [System.Serializable]
        public struct Main
        {
            public string terminalWelcome;
            public string selectEntries;
            public string emails_btn;
            public string logs_btn;
            public string exit_btn;
        }
        public Mail mail;

        [System.Serializable]
        public struct Mail
        {
            public string header;
            public string mailCounter;
        }
        public Logs logs;

        [System.Serializable]
        public struct Logs
        {
            public string header;
        }
    }
}