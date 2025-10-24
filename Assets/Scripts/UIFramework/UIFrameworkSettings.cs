using System.Collections.Generic;
using UnityEngine;

namespace UIFramework
{
    [CreateAssetMenu(fileName = "New UIFramework Settings", menuName = "UIFramework")]
    public class UIFrameworkSettings : ScriptableObject
    {
        [Header("UI预制体")]
        public List<GameObject> uiToRegister;

        [Header("初始化处于开启状态的UI")] 
        public List<string> uiToEnableAtStart;
    }
}