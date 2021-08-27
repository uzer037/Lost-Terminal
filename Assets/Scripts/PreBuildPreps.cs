
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
class PreBuildPreps : IPreprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }
    public void OnPreprocessBuild(BuildReport report)
    {
        List<Entry> entries = new List<Entry>(Resources.LoadAll<Entry>("Entries"));
        TerminalData data = Resources.Load<TerminalData>("TerminalData");
        data.entries = entries;
        Debug.Log("Build stage: Updated Entries List.");

        EditorUtility.SetDirty(data);
        AssetDatabase.SaveAssets();
    }
}

#endif