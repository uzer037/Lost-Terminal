using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Lean.Localization.LeanPhrase))]
public class EntryLocalisation : MonoBehaviour
{
    public Lean.Localization.LeanPhrase phrase;
    public List<Entry> entries;

    public void Init()
    {
        phrase = gameObject.GetComponent<Lean.Localization.LeanPhrase>();
            entries = new List<Entry>();
            Debug.Log(phrase.Entries.Count);

            foreach (var lang in Lean.Localization.LeanLocalization.CurrentLanguages.Keys)
            {
                entries.Add(ScriptableObject.CreateInstance<Entry>());
            }
            UpdateEntry();
    }

    public void UpdateEntry()
    {
        int iter = 0;
        foreach (var lang in Lean.Localization.LeanLocalization.CurrentLanguages.Keys)
        {
            phrase.AddEntry(lang, obj: entries[iter]);
            iter++;
        }
    }
}

#if UNITY_EDITOR
namespace EntryEditorNamespace
{
    using UnityEditor;
    [CustomEditor(typeof(EntryLocalisation))]
    class EntryEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            int iter = 0;
            string[] keys = new string[Lean.Localization.LeanLocalization.CurrentLanguages.Keys.Count];
            Lean.Localization.LeanLocalization.CurrentLanguages.Keys.CopyTo(keys, 0);
            EditorGUILayout.LabelField("Entry: " + (target as EntryLocalisation).gameObject.name.Remove(0,6)); // "Entry_" length is 6
            EditorGUILayout.Separator();
            foreach (Entry obj in (target as EntryLocalisation).entries)
            {
                EditorGUILayout.LabelField(keys[iter]);
                EditorGUI.indentLevel++;
                obj.title = EditorGUILayout.TextField(obj.title);
                obj.value = EditorGUILayout.TextArea(obj.value);
                EditorUtility.SetDirty(obj);
                EditorGUI.indentLevel--;
                iter++;
            }
        }
    }
}
#endif