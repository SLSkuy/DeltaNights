using System;
using UnityEngine;

namespace UIFramework.Window
{
    /// <summary>
    /// 窗口属性
    /// 可以创建新的对象覆写窗口原属性
    /// </summary>
    [Serializable]
    public class WindowProperties : IWindowProperties
    {
        [SerializeField]
        protected WindowPriority priority = WindowPriority.ForceForeground;
        [SerializeField]
        protected bool hideOnForegroundLost = true;
        [SerializeField]
        protected bool isPopup = false;
        
        #region 暴露属性

        /// <summary>
        /// 如果另一个窗口已经打开，当前窗口要如何处理
        /// </summary>
        /// <value>ForceForeground 立即打开显示；Enqueue 加入队列，在合适的时候打开</value>
        public WindowPriority Priority { get => priority; set => priority = value; }
        
        /// <summary>
        /// 当前窗口被其他窗口覆盖时，是否隐藏当前窗口
        /// </summary>
        public bool HideOnForegroundLost { get => hideOnForegroundLost; set => hideOnForegroundLost = value; }

        /// <summary>
        /// 当通过带参数显示UI方法调用时，是否覆盖窗口原有属性
        /// </summary>
        public bool SuppressPrefabProperties { get; set; }

        /// <summary>
        /// 是否为弹窗，显示在所有窗口之上并启用蒙黑覆盖其他窗口
        /// </summary>
        public bool IsPopup { get => isPopup; set => isPopup = value; }

        #endregion

        public WindowProperties(WindowPriority priority = WindowPriority.ForceForeground,
            bool hideOnForegroundLost = true, bool isPopup = false)
        {
            this.priority = priority;
            this.hideOnForegroundLost = hideOnForegroundLost;
            this.isPopup = isPopup;
        }
    }
}