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
        /// 带蒙黑
        /// </summary>
        [SerializeField] private WindowPriorityLayer priorityLayerWindow;

        public IWindowController CurrentWindow { get; private set; }

        private List<string> _readyToShow;
        private Queue<WindowHistoryEntry> _windowQueue;
        private Stack<WindowHistoryEntry> _windowHistory;

        public event Action RequestedScreenBlock;
        public event Action RequestedScreenUnBlock;
        
        #endregion
        
        #region 窗口控制器管理方法
        
        public override void ShowUI(IWindowController controller)
        {
            if(ShouldEnqueue(controller))
            {
                // 当前队列已经存在对应窗口，不再加入队列
                if (_readyToShow.Contains(controller.UIControllerID) || CurrentWindow == controller)
                {
                    Debug.LogWarning($"[WindowLayer] {controller.UIControllerID} is already in queue or showing");
                    return;
                }
                
                Enqueue(controller);
            }
            else
            {
                DoShow(controller);
            }
        }

        public override void HideUI(IWindowController controller)
        {
            if (controller == CurrentWindow)
            {
                CurrentWindow = null;
                _windowHistory.Pop();
                _readyToShow.Remove(controller.UIControllerID);
                BlockScreen(controller);
                controller.Hide();

                if (_windowQueue.Count > 0)
                {
                    ShowNextInQueue();
                }
                else if(_windowHistory.Count > 0)
                {
                    ShowPreviousInHistory();
                }
            }
            else
            {
                Debug.LogError($"[WindowLayer] Hide requested on WindowID {controller.UIControllerID}, but it is not the current Window");
            }
        }

        public override void HideAllUI(bool isAnimate = true)
        {
            base.HideAllUI(isAnimate);
            CurrentWindow = null;
            priorityLayerWindow.RefreshDarken();
            _windowHistory.Clear();
        }

        #endregion
        
        #region 窗口层管理方法

        public override void Initialize()
        {
            base.Initialize();
            _windowQueue = new Queue<WindowHistoryEntry>();
            _windowHistory = new Stack<WindowHistoryEntry>();
            _readyToShow = new List<string>();
        }
        
        public override void ReParentUI(IUIController controller, Transform uiTransform)
        {
            // 判断是否为弹窗，若是则添加到辅助层进行管理
            if (controller is IWindowController { IsPopup: true })
            {
                priorityLayerWindow.AddUI(uiTransform);
            }
            else if(controller is IWindowController)
            {
                // 普通窗口
                base.ReParentUI(controller,uiTransform);
            }
            else
            {
                Debug.LogError($"[WindowLayer] ReParent failed, controller is null");
            }
        }

        protected override void ProcessUIRegister(string uiControllerID, IWindowController controller)
        {
            base.ProcessUIRegister(uiControllerID, controller);
            controller.InTransitionFinished += OnInAnimationFinished;
            controller.OutTransitionFinished += OnOutAnimationFinished;
            controller.CloseRequested += OnCloseRequested;
        }

        protected override void ProcessUIUnregister(string uiControllerID, IWindowController controller)
        {
            base.ProcessUIUnregister(uiControllerID, controller);
            controller.InTransitionFinished -= OnInAnimationFinished;
            controller.OutTransitionFinished -= OnOutAnimationFinished;
            controller.CloseRequested -= OnCloseRequested;
        }

        private void OnCloseRequested(IUIController controller)
        {
            HideUI(controller as IWindowController);
        }
        
        /// <summary>
        /// 判断窗口优先级决定是否加入待显示队列
        /// </summary>
        /// <param name="controller">窗口控制器</param>
        /// <returns>是否应该加入待显示队列</returns>
        private bool ShouldEnqueue(IWindowController controller)
        {
            if (CurrentWindow == null && _windowQueue.Count == 0)
            {
                return false;
            }

            if (controller.Priority == WindowPriority.Enqueue)
            {
                return true;
            }

            return false;
        }

        private void Enqueue(IWindowController controller)
        {
            _readyToShow.Add(controller.UIControllerID);
            _windowQueue.Enqueue(new WindowHistoryEntry(controller));
        }

        private void ShowNextInQueue()
        {
            if (_windowQueue.Count > 0)
            {
                WindowHistoryEntry entry = _windowQueue.Dequeue();
                DoShow(entry.Controller);
            }
        }

        private void ShowPreviousInHistory()
        {
            if (_windowHistory.Count > 0)
            {
                WindowHistoryEntry entry = _windowHistory.Pop();
                DoShow(entry.Controller);
            }
        }

        /// <summary>
        /// 处理窗口显示逻辑
        /// </summary>
        /// <param name="controller">窗口控制器</param>
        private void DoShow(IWindowController controller)
        {
            if (controller == CurrentWindow)
            {
                Debug.LogWarning($"[WindowLayer] {controller.UIControllerID} is already show");
                return;
            }
            
            if(CurrentWindow != null && CurrentWindow.HideOnForegroundLost && !controller.IsPopup)
            {
                CurrentWindow.Hide();
            }
            
            // 将当前窗口加载到窗口历史中
            _windowHistory.Push(new WindowHistoryEntry(controller));
            BlockScreen(controller);

            // 启用蒙黑层
            if (controller.IsPopup)
            {
                priorityLayerWindow.DarkenBg();
            }
            
            controller.Show();  // 委托Controller进行窗口显示
            CurrentWindow = controller;
        }
        
        /// <summary>
        /// 窗口动画过渡时禁止额外点击操作
        /// </summary>
        private void BlockScreen(IUIController controller)
        {
            RequestedScreenBlock?.Invoke();
        }

        /// <summary>
        /// 窗口动画过渡完毕时恢复点击操作
        /// </summary>
        private void UnBlockScreen(IUIController controller)
        {
            RequestedScreenUnBlock?.Invoke();
        }
        
        /// <summary>
        /// 进入窗口动画播放完毕回调
        /// </summary>
        private void OnInAnimationFinished(IUIController controller)
        {
            UnBlockScreen(controller);
        }

        /// <summary>
        /// 隐藏窗口动画播放完毕回调
        /// </summary>
        private void OnOutAnimationFinished(IUIController controller)
        {
            UnBlockScreen(controller);
            if (controller is IWindowController { IsPopup: true })
            {
                priorityLayerWindow.RefreshDarken();
            }
        }
        
        #endregion
    }
}