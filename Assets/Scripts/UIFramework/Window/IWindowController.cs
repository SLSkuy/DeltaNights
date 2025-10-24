using UIFramework.Core;

namespace UIFramework.Window
{
    /// <summary>
    /// 窗口UI控制器接口
    /// 在UI界面控制器的基础上声明窗口UI特有的属性与方法
    /// 实现该接口即为窗口的逻辑层
    /// </summary>
    public interface IWindowController : IUIController
    {
        WindowPriority Priority { get; }    // 窗口优先级
        bool HideOnForegroundLost { get;}   // 当前窗口被其他窗口覆盖时，是否隐藏当前窗口
        bool IsPopup { get;}    // 是否是弹窗
    }
}