using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Lean.Localization;

namespace CustomLocalization
{
    [RequireComponent(typeof(Lean.Localization.LeanLocalization))]
    public class TerminalLocalisation : MonoBehaviour
    {
        public static GameObject localizationObject;

        void Awake()
        {
            if(localizationObject == null)
                localizationObject = this.gameObject;
        }
    }
}