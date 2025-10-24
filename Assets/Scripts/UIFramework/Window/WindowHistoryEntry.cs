using System;

namespace UIFramework.Window
{
    /// <summary>
    /// 窗口历史记录
    /// 用于存储历史窗口的信息，便于控制对应窗口
    /// </summary>
    [Serializable]
    public class WindowHistoryEntry
    {
        private IWindowController _windowController;
        private IWindowProperties _windowProperties;

        #region 暴露属性

        public IWindowController WindowController { get =>  _windowController; set => _windowController = value; }
        public IWindowProperties WindowProperties { get => _windowProperties; set => _windowProperties = value; }

        #endregion

        public WindowHistoryEntry(IWindowController windowController, IWindowProperties windowProperties)
        {
            _windowController = windowController;
            _windowProperties = windowProperties;
        }
    }
}