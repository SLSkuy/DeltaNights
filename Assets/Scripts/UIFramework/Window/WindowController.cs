using UIFramework.Core;
using UnityEngine;

namespace UIFramework.Window
{
    /// <summary>
    /// 窗口控制器，控制窗口的各种行为
    /// </summary>
    public class WindowController : UIController<IWindowProperties>, IWindowController
    {
        #region 暴露属性

        public WindowPriority Priority => Properties?.Priority ?? WindowPriority.ForceForeground;
        public bool HideOnForegroundLost => Properties?.HideOnForegroundLost ?? true;
        public bool IsPopup => Properties?.IsPopup ?? false;
        
        #endregion
        
        #region 控制器方法

        protected override void HierarchyFixOnShow()
        {
            transform.SetAsLastSibling();
        }

        protected override void SetProperties(IWindowProperties props)
        {
            if (props != null)
            {
                props.Priority = Priority;
                props.HideOnForegroundLost = HideOnForegroundLost;
                props.IsPopup = IsPopup;
            }
            base.SetProperties(props);
        }

        /// <summary>
        /// 关闭当前窗口，使用UI_前缀区分方法
        /// UI_前缀供Inspector上配置使用
        /// </summary>
        public virtual void UI_Close()
        {
            CloseUIRequested(this);
        }
        
        #endregion
    }
}