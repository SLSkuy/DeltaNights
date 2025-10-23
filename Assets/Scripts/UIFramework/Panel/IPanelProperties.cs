using UIFramework.Core;

namespace UIFramework.Panel
{
    /// <summary>
    /// 面板属性接口
    /// 声明所有面板应有的属性
    /// 实现该类用于定义面板的属性，以确定面板的显示效果
    /// </summary>
    public interface IPanelProperties : IUIProperties
    {
        PanelPriority Priority { get; set; }
    }
}