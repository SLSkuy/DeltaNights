namespace UIFramework.Panel
{
    /// <summary>
    /// 定义面板的优先级
    /// </summary>
    public enum PanelPriority
    {
        /// <summary>
        /// 无优先级
        /// </summary>
        None,
        /// <summary>
        /// 初等优先
        /// </summary>
        Priority,
        /// <summary>
        /// 指引面板
        /// </summary>
        Tutorial,
        /// <summary>
        /// 阻挡遮罩面板
        /// </summary>
        Blocker
    }
}