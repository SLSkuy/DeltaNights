using System;
using UnityEngine;

namespace UIFramework.Panel
{
    /// <summary>
    /// 面板属性
    /// </summary>
    [Serializable]
    public class PanelProperties : IPanelProperties
    {
        [SerializeField] [Tooltip("当前面板优先级，面板会根据优先级划分到不同的子层管理")]
        private PanelPriority priority;

        public PanelPriority Priority { get => priority;set => priority = value; }
    }
}