using System;
using System.Collections.Generic;
using UnityEngine;

namespace UIFramework.Panel
{
    /// <summary>
    /// 根据优先级划分面板层的子层
    /// 使用列表存储各子层
    /// </summary>
    [Serializable]
    public class PanelPriorityLayerList
    {
        [Header("面板层优先级子层列表")]
        [SerializeField] [Tooltip("配置各个子层的优先级及父节点，渲染优先级由Inspector中的顺序决定")]
        private List<PanelPriorityLayerEntry> subLayers;

        private Dictionary<PanelPriority, Transform> _lookupList;

        #region 暴露属性

        public Dictionary<PanelPriority, Transform> LookupList
        {
            get
            {
                // 如果查找字典值为空或未进行初始化，读取在Inspector窗口中配置的subLayers的值进行初始化
                if (_lookupList == null || _lookupList.Count == 0)
                {
                    // 检测Count是为了应对初始未给list赋值而是在运行时通过编辑器添加，此时需要刷新数据
                    CacheLookup();
                }
                return _lookupList;
            }
        }

        #endregion
        
        /// <summary>
        /// 如果当前查找字典为空或未初始化，则从subLayers中读取子层信息并添加到字典中
        /// </summary>
        private void CacheLookup()
        {
            _lookupList = new Dictionary<PanelPriority, Transform>();
            foreach (var sublayer in subLayers)
            {
                _lookupList.Add(sublayer.Priority, sublayer.LayerRootTransform);
            }
        }

        public PanelPriorityLayerList(List<PanelPriorityLayerEntry> subLayers)
        {
            this.subLayers = subLayers;
        }
    }
}