using System;
using System.Collections.Generic;
using UIFramework.Core;
using UnityEngine;

namespace UIFramework.Window
{
    /// <summary>
    /// 窗口（Window）的Layer
    /// 通过访问管理对象的controller进行窗口的控制
    /// 窗口是一种有历史记录和顺序的UI界面
    /// 通过栈和队列的结构对不同优先级的窗口进行存储
    /// </summary>
    public class WindowLayer : UIBaseLayer<IWindowController>
    {
        #region 内部成员

        /// <summary>
        /// 辅助界面用于显示弹窗等窗口
        /// </summary>
        [SerializeField] private WindowPriorityLayer priorityLayer;

        public IWindowController CurrentWindow { get; private set; }
        
        private Queue<WindowHistoryEntry> _windowQueue;
        private Stack<WindowHistoryEntry> _windowHistory;
        private HashSet<IUIController> _uiTransitioning;

        public event Action RequestedScreenBlock;
        public event Action RequestedScrennUnBlock;
        
        #endregion
        
        #region 窗口控制器方法

        public override void ShowUI(IWindowController controller) => controller.Show();
        public override void ShowUI<TProps>(IWindowController controller, TProps props) => controller.Show(props);
        public override void HideUI(IWindowController controller) => controller.Hide();

        #endregion
        
        #region 窗口管理方法

        public override void Initialize()
        {
            base.Initialize();
            _windowQueue = new Queue<WindowHistoryEntry>();
            _windowHistory = new Stack<WindowHistoryEntry>();
            _uiTransitioning = new HashSet<IUIController>();
        }
        
        public override void ReParentUI(IUIController controller, Transform uiTransform)
        {
            
        }

        #endregion
    }
}