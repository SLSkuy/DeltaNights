using UIFramework.Core;

namespace UIFramework.Window
{
    /// <summary>
    /// 窗口控制器
    /// </summary>
    public class WindowController : UIController<IWindowProperties>, IWindowController
    {
        #region 暴露属性
        
        public WindowPriority Priority => Properties.Priority;
        public bool HideOnForegroundLost => Properties.HideOnForegroundLost;
        public bool IsPopup => Properties.IsPopup;
        
        #endregion
        
        #region 控制器方法

        protected sealed override void SetProperties(IWindowProperties properties)
        {
            if (properties != null)
            {
                properties.Priority = Priority;
                properties.HideOnForegroundLost = HideOnForegroundLost;
                properties.IsPopup = IsPopup;
            }
            base.SetProperties(properties);
        }

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