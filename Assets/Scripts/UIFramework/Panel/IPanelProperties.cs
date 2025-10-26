using UIFramework.Core;

namespace UIFramework.Panel
{
    /// <summary>
    /// 面板界面属性
    /// </summary>
    public interface IPanelProperties : IUIProperties
    {
        PanelPriority Priority { get; set; }
    }
}