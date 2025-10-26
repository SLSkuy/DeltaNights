using System;
using UnityEngine;

namespace UIFramework.Window
{
    [Serializable]
    public class WindowProperties : IWindowProperties
    {
        [SerializeField] [Tooltip("当前窗口优先级，判断窗口的显示时机")]
        private WindowPriority priority;
        [SerializeField] [Tooltip("窗口被覆盖时是否隐藏")]
        private bool hideOnForegroundLost;
        [SerializeField] [Tooltip("若为弹窗将移动到辅助层进行管理")]
        private bool isPopup;
        
        #region 暴露属性

        public WindowPriority Priority { get => priority; set => priority = value; }
        public bool HideOnForegroundLost { get => hideOnForegroundLost; set => hideOnForegroundLost = value; }
        public bool IsPopup { get => isPopup; set => isPopup = value; }

        #endregion

        public WindowProperties(WindowPriority priority, bool hideOnForegroundLost, bool isPopup)
        {
            this.priority = priority;
            this.hideOnForegroundLost = hideOnForegroundLost;
            this.isPopup = isPopup;
        }
    }
}