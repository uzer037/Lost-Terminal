using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TranslationData))]
class TranslationDataEditor : Editor
{

    public override void OnInspectorGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Current Language: ");
        GUIContent langsDropdown = new GUIContent(TranslationData.languageNames[TranslationData.currentLanguage]);
        string[] langs = new string[TranslationData.languageNames.Count];
        TranslationData.languageNames.Values.CopyTo(langs, 0);
        TranslationData.currentLanguage = (TranslationData.Language)EditorGUILayout.Popup((int)TranslationData.currentLanguage, langs);
        GUILayout.EndHorizontal();
        base.OnInspectorGUI();
    }
}

[CustomEditor(typeof(Translation))]
public class TranslationEditor : Editor
{
    Translation lastObj;
    bool showEditorData = true;
    public override void OnInspectorGUI()
    {
        showEditorData = EditorGUILayout.Foldout(showEditorData, "Editor Data");
        Translation newObj = (Translation)target;
        SerializedObject serializedObj = new SerializedObject(newObj);

        if(showEditorData)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObj.FindProperty("language"));
            EditorGUI.indentLevel--;
        }

        if (newObj != null)
        {
            if(showEditorData)
            {
                EditorGUI.indentLevel++;
                DrawProperties(serializedObj.FindProperty("editorData"), true);
                EditorGUI.indentLevel--;
            }
                EditorGUILayout.Separator();
            DrawProperties(serializedObj.FindProperty("ui"), true);
        }
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
            serializedObj.ApplyModifiedProperties();
        }

        lastObj = newObj;
    }

    private void DrawProperties(SerializedProperty prop, bool drawChildren)
    {
        string lastPropPath = string.Empty;
        foreach (SerializedProperty p in prop)
        {
            if (p.isArray && p.propertyType == SerializedPropertyType.Generic)
            {
                EditorGUILayout.BeginHorizontal();
                p.isExpanded = EditorGUILayout.Foldout(p.isExpanded, p.displayName);
                EditorGUILayout.EndHorizontal();

                if (p.isExpanded)
                {
                    EditorGUI.indentLevel++;
                    DrawProperties(p, drawChildren);
                    EditorGUI.indentLevel--;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(lastPropPath) && p.propertyPath.Contains(lastPropPath)) { continue; }
                lastPropPath = p.propertyPath;
                EditorGUILayout.PropertyField(p, drawChildren);
            }
        }
    }
}
public class TranslationEditorWindow : ExtendedEditorWindow
{
    protected Translation obj;
    static TranslationEditorWindow window;
    public static void Open(Translation translation)
    {
        window = GetWindow<TranslationEditorWindow>("Translation Editor");
        window.obj = translation;
        window.serializedObject = new SerializedObject(translation);
    }
    public static void UpdateObject(Translation translation)
    {
        if (window != null)
        {
            window.obj = translation;
            window.serializedObject = new SerializedObject(translation);
        }
    }

    private void OnGUI()
    {
        if (serializedObject != null)
        {
            GUILayout.Label("Name: ");
            GUILayout.BeginHorizontal();
            GUILayout.Label("UI");
            currentProperty = serializedObject.FindProperty("ui");
            GUILayout.EndHorizontal();
            DrawProperties(currentProperty, true);
        }
    }
}