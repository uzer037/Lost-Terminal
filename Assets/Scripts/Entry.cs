using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Entry", menuName = "ScriptableObjects/Entry", order = 2)]
[System.Serializable]
public class Entry : ScriptableObject
{
    public string title;
    public string value;
}