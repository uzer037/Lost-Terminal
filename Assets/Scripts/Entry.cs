using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
 public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
 {
     [SerializeField]
     private List<TKey> keys = new List<TKey>();
     
     [SerializeField]
     private List<TValue> values = new List<TValue>();
     
     // save the dictionary to lists
     public void OnBeforeSerialize()
     {
         keys.Clear();
         values.Clear();
         foreach(KeyValuePair<TKey, TValue> pair in this)
         {
             keys.Add(pair.Key);
             values.Add(pair.Value);
         }
     }
     
     // load dictionary from lists
     public void OnAfterDeserialize()
     {
         this.Clear();
 
         if(keys.Count != values.Count)
             throw new System.Exception(string.Format("there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable."));
 
         for(int i = 0; i < keys.Count; i++)
             this.Add(keys[i], values[i]);
     }
 }

[Serializable]
public class TranslatedText : SerializableDictionary<TranslationData.Language, string>{}

[CreateAssetMenu(fileName = "Entry", menuName = "ScriptableObjects/Entry", order = 3)]
public class Entry : ScriptableObject
{
    // Start is called before the first frame update
    public string UID
    {
        get{return _UID;}
    }
    private string _UID;
        private void OnValidate()
        {
    #if UNITY_EDITOR
            if (_UID == "")
            {
                _UID = GUID.Generate().ToString();
                UnityEditor.EditorUtility.SetDirty(this);
            }
    #endif
        }
    public TranslatedText title;
    public TranslatedText text;
}
