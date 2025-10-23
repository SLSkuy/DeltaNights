using System;
using UnityEngine;

namespace UIFramework.Panel
{
    /// <summary>
    /// 面板优先级子层对象
    /// 定义各子层的属性
    /// </summary>
    [Serializable]
    public class PanelPriorityLayerEntry
    {
        [Header("面板优先级子层属性")]
        [SerializeField] [Tooltip("指定当前子面板层的优先级")]
        private PanelPriority priority;
        [SerializeField] [Tooltip("指定当前子面板层管理的所有子对象的父节点")]
        private Transform subLayerRootTransformTransform;
        
        #region 暴露属性

        public Transform LayerRootTransform { get => subLayerRootTransformTransform; set => subLayerRootTransformTransform = value; }
        public PanelPriority Priority { get => priority; set => priority = value; }
        
        #endregion

        public PanelPriorityLayerEntry(PanelPriority priority, Transform subLayerRootTransformTransform)
        { 
            this.priority = priority;
            this.subLayerRootTransformTransform = subLayerRootTransformTransform;
        }
    }
}