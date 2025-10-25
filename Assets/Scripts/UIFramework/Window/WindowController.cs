using UIFramework.Core;
using UnityEngine;

namespace UIFramework.Window
{
    /// <summary>
    /// 窗口控制器，控制窗口的各种行为
    /// </summary>
    public class WindowController : UIController, IWindowController
    {
        [SerializeField] [Tooltip("当前窗口优先级，判断窗口的显示时机")]
        private WindowPriority priority;
        [SerializeField] [Tooltip("窗口被覆盖时是否隐藏")]
        private bool hideOnForegroundLost;
        [SerializeField] [Tooltip("若为弹窗将移动到辅助层进行管理")]
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
        /// UI_前缀供Inspector上配置使用
        /// </summary>
        public virtual void UI_Close()
        {
            CloseUIRequested(this);
        }

        /// <summary>
        /// 打开新的UI界面，使用UI_前缀区分方法
        /// UI_前缀供Inspector上配置使用
        /// </summary>
        /// <param name="id">要打开的UI界面ID</param>
        public virtual void UI_Open(string id)
        {
            UIFramework.Instance.ShowUI(id);
        }
        
        #endregion
    }
}