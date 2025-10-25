using UIFramework.Core;
using UnityEngine;

namespace UIFramework.Panel
{
    /// <summary>
    /// 面板控制器基类，控制面板的各种行为
    /// </summary>
    public class PanelController : UIController, IPanelController
    {
        [SerializeField] [Tooltip("当前面板优先级，面板会根据优先级划分到不同的子层管理")]
        private PanelPriority priority;
        
        #region 暴露属性

        public PanelPriority Priority => priority;

        #endregion
    }
}