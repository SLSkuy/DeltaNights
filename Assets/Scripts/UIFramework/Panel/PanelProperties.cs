using System;
using UnityEngine;

namespace UIFramework.Panel
{
    [Serializable]
    public class PanelProperties : IPanelProperties
    {
        [SerializeField] [Tooltip("当期面板优先级，决定渲染优先级")]
        private PanelPriority priority;
        
        #region 暴露属性
        
        public PanelPriority Priority { get => priority; set => priority = value; }
        
        #endregion

        public PanelProperties(PanelPriority priority)
        {
            this.priority = priority;
        }
    }
}