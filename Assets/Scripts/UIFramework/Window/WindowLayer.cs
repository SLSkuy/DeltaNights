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
        
        private Queue<WindowHistoryEntry> _windowQueue;
        private Stack<WindowHistoryEntry> _windowHistory;
        
        private HashSet<IUIController> _uiTransitions;
        private bool _isUITransitionInProgress;

        public event Action RequestedScreenBlock;
        public event Action RequestedScreenUnBlock;
        
        #endregion
        
        #region 窗口控制器管理方法
        
        public override void ShowUI(IWindowController controller)
        {
            ShowUI<IWindowProperties>(controller, null);    
        }

        public override void ShowUI<TProps>(IWindowController controller, TProps props)
        {
            IWindowProperties windowProperties = props as IWindowProperties;
            if(ShouldEnqueue(controller, windowProperties))
            {
                Enqueue(controller, windowProperties);
            }
            else
            {
                DoShow(controller, windowProperties);
            }
        }

        public override void HideUI(IWindowController controller)
        {
            if (controller == CurrentWindow)
            {
                CurrentWindow = null;
                _windowHistory.Pop();
                AddTransition(controller);
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
            _uiTransitions = new HashSet<IUIController>();
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
        
        private bool ShouldEnqueue(IWindowController controller, IWindowProperties properties)
        {
            if (CurrentWindow == null && _windowQueue.Count == 0)
            {
                return false;
            }

            // 是否使用覆写属性
            if (properties != null && properties.SuppressPrefabProperties)
            {
                return properties.Priority == WindowPriority.Enqueue;
            }

            if (controller.Priority == WindowPriority.Enqueue)
            {
                return true;
            }

            return false;
        }

        private void Enqueue(IWindowController controller, IWindowProperties properties)
        {
            _windowQueue.Enqueue(new WindowHistoryEntry(controller, properties));
        }

        private void ShowNextInQueue()
        {
            if (_windowQueue.Count > 0)
            {
                WindowHistoryEntry entry = _windowQueue.Dequeue();
                DoShow(entry.WindowController, entry.WindowProperties);
            }
        }

        private void ShowPreviousInHistory()
        {
            if (_windowHistory.Count > 0)
            {
                WindowHistoryEntry entry = _windowHistory.Pop();
                DoShow(entry.WindowController, entry.WindowProperties);
            }
        }

        /// <summary>
        /// 处理窗口显示逻辑
        /// </summary>
        /// <param name="controller">窗口控制器</param>
        /// <param name="properties">窗口属性</param>
        private void DoShow(IWindowController controller, IWindowProperties properties)
        {
            if (controller == CurrentWindow)
            {
                Debug.LogWarning($"[WindowLayer] {controller.UIControllerID} is already show");
            }
            else if(CurrentWindow != null && CurrentWindow.HideOnForegroundLost && !controller.IsPopup)
            {
                CurrentWindow.Hide();
            }
            
            // 将当前窗口加载到窗口历史中
            _windowHistory.Push(new WindowHistoryEntry(controller, properties));
            AddTransition(controller);

            // 启用蒙黑层
            if (controller.IsPopup)
            {
                priorityLayerWindow.DarkenBg();
            }
            
            controller.Show();
            CurrentWindow = controller;
        }
        
        /// <summary>
        /// 添加待处理窗口动画
        /// </summary>
        private void AddTransition(IUIController controller)
        {
            _uiTransitions.Add(controller);
            RequestedScreenBlock?.Invoke();
        }

        /// <summary>
        /// 移除窗口动画
        /// </summary>
        private void RemoveTransition(IUIController controller)
        {
            _uiTransitions.Remove(controller);
            if (!_isUITransitionInProgress)
            {
                RequestedScreenUnBlock?.Invoke();
            }
        }
        
        /// <summary>
        /// 进入窗口动画播放完毕回调
        /// </summary>
        private void OnInAnimationFinished(IUIController controller)
        {
            RemoveTransition(controller);
        }

        /// <summary>
        /// 隐藏窗口动画播放完毕回调
        /// </summary>
        private void OnOutAnimationFinished(IUIController controller)
        {
            RemoveTransition(controller);
            if (controller is IWindowController { IsPopup: true })
            {
                priorityLayerWindow.RefreshDarken();
            }
        }
        
        #endregion
    }
}