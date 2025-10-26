using System;
using System.Collections.Generic;
using UnityEngine;

namespace UIFramework
{
    [Serializable]
    public struct UISettingsEntry
    {
        public bool isEnableOnRegister;
        public GameObject uiPrefab;
    }
    
    [CreateAssetMenu(fileName = "New UIFramework Settings", menuName = "UIFramework")]
    public class UIFrameworkSettings : ScriptableObject
    {
        [Header("UI To Register List")]
        public List<UISettingsEntry> uiToRegister;
    }
}