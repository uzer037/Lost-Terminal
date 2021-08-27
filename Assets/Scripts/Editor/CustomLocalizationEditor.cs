#if UNITY_EDITOR
namespace CustomLocalization.Editor
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;
    using CustomLocalization;
    using Lean.Localization.Editor;
    [CustomEditor(typeof(CustomLocalization.TerminalLocalisation))]
    public class LocalisationEditor : LeanLocalization_Editor
    {
        Vector2 entryScroll;
        Vector2 menuTextScroll;
        string entryTitle;
        string menuText;
        string entrySearchbox = "";
        string menuTextSearchbox = "";
        bool showEntriesDropdown = true;
        bool showMenuTextDropdown = true;

        List<Lean.Localization.LeanPhrase> phrases;

        [MenuItem("GameObject/Lean/Back To Localisation %#l")]
        public static void BackToLocalisation()
        {
            Selection.activeObject = Lean.Localization.LeanLocalization.Instances[0];
            EditorGUIUtility.PingObject(Lean.Localization.LeanLocalization.Instances[0]);
        }
        void ReloadPhrases()
        {
            phrases = new List<Lean.Localization.LeanPhrase>((target as CustomLocalization.TerminalLocalisation).transform.GetComponentsInChildren<Lean.Localization.LeanPhrase>());
        }
        protected override void OnInspector()
        {
            if (phrases == null)
                ReloadPhrases();

            GUILayout.BeginHorizontal();
            entryTitle = GUILayout.TextField(entryTitle);
            bool addEntry = GUILayout.Button("Add Entry");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            menuText = GUILayout.TextField(menuText);
            bool addMenuText = GUILayout.Button("Add MenuText");
            GUILayout.EndHorizontal();

            //  ADD ENTRY
            if (addEntry)
            {
                string title = "Entry_" + entryTitle;
                GameObject obj = Lean.Localization.LeanLocalization.Instances[0].gameObject;
                Transform phraseTransform = obj.transform.Find(title);
                Lean.Localization.LeanPhrase phrase;
                if (phraseTransform == null)
                {
                    phrase = Lean.Localization.LeanLocalization.AddPhraseToFirst(title);
                    phrase.Data = Lean.Localization.LeanPhrase.DataType.Object;
                    phrase.gameObject.AddComponent<EntryLocalisation>().Init();
                    ReloadPhrases();

                    Lean.Localization.LeanLocalization.UpdateTranslations();

                }
                else
                {
                    phrase = phraseTransform.gameObject.GetComponent<Lean.Localization.LeanPhrase>();
                    Debug.LogWarning("Translations list already contains this key.");
                }
                    Selection.activeObject = phrase.gameObject;
                UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(phrase, false);
            }

            //  ADD PHRASE
            if (addMenuText)
            {
                string title = "Phrase_" + menuText;
                GameObject obj = Lean.Localization.LeanLocalization.Instances[0].gameObject;
                Transform phraseTransform = obj.transform.Find(title);
                Lean.Localization.LeanPhrase phrase;
                if (phraseTransform == null)
                {
                    phrase = Lean.Localization.LeanLocalization.AddPhraseToFirst(title);
                    ReloadPhrases();

                    Lean.Localization.LeanLocalization.UpdateTranslations();

                }
                else
                {
                    phrase = phraseTransform.gameObject.GetComponent<Lean.Localization.LeanPhrase>();
                    Debug.LogWarning("Translations list already contains this key.");
                }
                Selection.activeObject = phrase.gameObject;
                UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(phrase, true);
            }

            showEntriesDropdown = EditorGUILayout.Foldout(showEntriesDropdown, "Entries list");
            if (showEntriesDropdown)
            {
                EditorGUI.indentLevel++;
                entrySearchbox = GUILayout.TextField(entrySearchbox);

                entryScroll = EditorGUILayout.BeginScrollView(entryScroll, false, true);
                EditorGUILayout.Space(10);
                if (phrases != null)
                {
                    bool hasMathcing = false;
                    bool foundTranslation = false;
                    foreach (var phrase in phrases)
                    {
                        if (phrase.gameObject.name.StartsWith("Entry_"))
                        {
                            foundTranslation = true;
                            string name = phrase.gameObject.name.Remove(0, 6);// "Entry_" is 6 letters
                            if (entrySearchbox == "" || name.Contains(entrySearchbox))
                            {
                                if (GUILayout.Button(name))
                                {
                                    Selection.activeObject = phrase;
                                }
                                hasMathcing = true;
                            }
                        }
                    }
                    if (foundTranslation)
                    {
                        if (!hasMathcing)
                            EditorGUILayout.LabelField("No translations matching criteria.");
                    }
                    else
                    {
                        EditorGUILayout.LabelField("No translations added.");
                    }
                }
                

                EditorGUILayout.Space(10);
                EditorGUILayout.EndScrollView();
                EditorGUI.indentLevel--;
            }

            showMenuTextDropdown = EditorGUILayout.Foldout(showMenuTextDropdown, "Phrases list");
            if (showMenuTextDropdown)
            {
                EditorGUI.indentLevel++;
                menuTextSearchbox = GUILayout.TextField(menuTextSearchbox);

                menuTextScroll = EditorGUILayout.BeginScrollView(menuTextScroll, false, true);
                EditorGUILayout.Space(10);
                if (phrases != null)
                {
                    bool hasMathcing = false;
                    bool foundTranslation = false;
                    foreach (var phrase in phrases)
                    {
                        if (phrase.gameObject.name.StartsWith("Phrase_"))
                        {
                            foundTranslation = true;
                            string name = phrase.gameObject.name.Remove(0, 7);// "Phrase_" is 7 letters
                            if (menuTextSearchbox == "" || name.Contains(menuTextSearchbox))
                            {
                                if (GUILayout.Button(name))
                                {
                                    Selection.activeObject = phrase;
                                }
                                hasMathcing = true;
                            }
                        }
                    }

                    if (foundTranslation)
                    {
                        if (!hasMathcing)
                            EditorGUILayout.LabelField("No translations matching criteria.");
                    }
                    else
                    {
                        EditorGUILayout.LabelField("No translations added.");
                    }
                }
                EditorGUILayout.Space(10);
                EditorGUILayout.EndScrollView();
                EditorGUI.indentLevel--;
            }
        }
    }
}
#endif

