using UIFramework.Core;

namespace UIFramework.Window
{
    /// <summary>
    /// 窗口界面属性
    /// </summary>
    public interface IWindowProperties : IUIProperties
    {
        WindowPriority Priority { get; set; }
        bool HideOnForegroundLost { get; set; }
        bool IsPopup { get; set; }
    }
}