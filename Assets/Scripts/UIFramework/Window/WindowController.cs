using UIFramework.Core;
using UnityEngine;

namespace UIFramework.Window
{
    /// <summary>
    /// 窗口控制器
    /// </summary>
    public class WindowController : UIController, IWindowController
    {
        [SerializeField] [Header("窗口优先级")] [Tooltip("当前窗口优先级，判断窗口的显示时机")]
        private WindowPriority priority;
        [SerializeField] [Header("被其他窗口覆盖时是否隐藏")] [Tooltip("若为否，则与其他窗口叠加显示")]
        private bool hideOnForegroundLost;
        [SerializeField] [Header("是否为弹窗")] [Tooltip("若为弹窗将移动到辅助层进行管理")]
        private bool isPopup;
        
        #region 暴露属性
        
        public WindowPriority Priority => priority;
        public bool HideOnForegroundLost => hideOnForegroundLost;
        public bool IsPopup => isPopup;
        
        #endregion
        
        #region 控制器方法

        protected override void HierarchyFixOnShow()
        {
            transform.SetAsLastSibling();
        }

        /// <summary>
        /// 关闭当前窗口，使用UI_前缀区分方法
        /// </summary>
        public virtual void UI_Close()
        {
            CloseUIRequested(this);
        }
        
        #endregion
    }
}