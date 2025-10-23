using UIFramework.Core;

namespace UIFramework.Window
{
    /// <summary>
    /// 窗口属性接口
    /// 声明所有窗口应有的属性
    /// 实现该类用于定义窗口的属性，以确定窗口的显示效果
    /// </summary>
    public interface IWindowProperties : IUIProperties
    {
        WindowPriority Priority { get; set; }   // 窗口优先级
        bool HideOnForegroundLost { get; set; } // 是否能够被覆盖
        bool IsPopup { get; set; }  // 是否是弹窗
        bool SuppressPrefabProperties { get; set; } // 是否要覆盖当前窗口的属性
    }
}