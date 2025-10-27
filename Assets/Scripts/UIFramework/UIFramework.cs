using System;
using UIFramework.Core;
using UIFramework.Panel;
using UIFramework.Window;
using UnityEngine;
using UnityEngine.UI;

namespace UIFramework
{
    /// <summary>
    /// UI框架，声明所有的对外接口
    /// 充当UIManager的作用
    /// </summary>
    public class UIFramework : MonoBehaviour
    {
        #region 内部成员
        
        private UIFrameworkSettings _uiSettings;
        
        // UI类别层级管理器
        private PanelLayer _panelLayer;
        private WindowLayer _windowLayer;
        
        private Canvas _mainCanvas;
        private GraphicRaycaster _graphicRaycaster;
        
        #endregion
        
        #region 暴露属性
        
        public Canvas MainCanvas { get { if (!_mainCanvas)_mainCanvas = _mainCanvas.GetComponent<Canvas>(); return _mainCanvas; } }
        public Camera CanvasCamera => _mainCanvas.worldCamera;
        public UIFrameworkSettings UISettings { get => _uiSettings; set => _uiSettings = value; }

        #endregion
        
        #region 框架内部管理方法
        
        public void Initialize()
        {
            // 初始化Panel层级管理器
            if (!_panelLayer)
            {
                _panelLayer = GetComponentInChildren<PanelLayer>();
                if (_panelLayer)
                {
                    _panelLayer.Initialize();
                }
                else
                {
                    Debug.LogError("[UIFramework] UI Frame lacks Panel Layer]");
                }
            }
            
            // 初始化Window层级管理器
            if (!_windowLayer)
            {
                _windowLayer = GetComponentInChildren<WindowLayer>();
                if (_windowLayer)
                {
                    _windowLayer.Initialize();
                    _windowLayer.RequestedScreenBlock += BlockScreen;
                    _windowLayer.RequestedScreenUnBlock += UnblockScreen;
                }
                else
                {
                    Debug.LogError("[UIFramework] UI Frame lacks Window Layer]");
                }
            }
            
            _graphicRaycaster = GetComponent<GraphicRaycaster>();
            RegisterAllUIPrefab();
        }

        /// <summary>
        /// 实例化并注册所有UI界面
        /// </summary>
        private void RegisterAllUIPrefab()
        {
            foreach (var entry in _uiSettings.uiToRegister)
            {
                GameObject prefab = Instantiate(entry.uiPrefab);
                IUIController controller = prefab.GetComponent<IUIController>();
                RegisterUI(controller.UIControllerID, controller, prefab.transform);
                if(!entry.isEnableOnRegister)controller.Hide();
            }
        }
        
        #endregion
        
        #region 框架对外暴露方法

        private void BlockScreen()
        {
            _graphicRaycaster.enabled = false;
        }

        private void UnblockScreen()
        {
            _graphicRaycaster.enabled = true;
        }

        public void ShowPanel(string id)
        {
            _panelLayer.ShowUIByID(id);
        }

        public void ShowPanel<T>(string id, T p) where T : IUIProperties
        {
            _panelLayer.ShowUIByID(id, p);
        }

        public void HidePanel(string id)
        {
            _panelLayer.HideUIByID(id);
        }

        public void OpenWindow(string id)
        {
            _windowLayer.ShowUIByID(id);
        }

        public void OpenWindow<T>(string id, T p) where T : IUIProperties
        {
            _windowLayer.ShowUIByID(id, p);
        }

        public void CloseWindow(string id)
        {
            _windowLayer.HideUIByID(id);
        }

        public void CloseCurrentWindow()
        {
            if(_windowLayer.CurrentWindow != null)CloseWindow(_windowLayer.CurrentWindow.UIControllerID);
        }

        /// <summary>
        /// 根据传入的ID显示对应的UI界面，不分面板还是窗口
        /// </summary>
        /// <param name="id">UI界面ID</param>
        public void ShowUI(string id)
        {
            if (IsUIRegistered(id, out var type)) {
                if (type == typeof(IWindowController)) {
                    OpenWindow(id);
                }
                else if (type == typeof(IPanelController)) {
                    ShowPanel(id);
                }
            }
            else {
                Debug.LogError($"[UIFramework] Tried to open Screen id {id} but it's not registered as Window or Panel!");
            }
        }

        /// <summary>
        /// 根据传入的ID显示对应的UI界面，不分面板还是窗口，同时设置其属性
        /// </summary>
        /// <param name="id">UI界面ID</param>
        /// <param name="p">UI界面属性参数</param>
        /// <typeparam name="T">UI界面属性类型</typeparam>
        public void ShowUI<T>(string id, T p) where T : IUIProperties
        {
            if (IsUIRegistered(id, out var type)) {
                if (type == typeof(IWindowController)) {
                    OpenWindow(id, p);
                }
                else if (type == typeof(IPanelController)) {
                    ShowPanel(id, p);
                }
            }
            else {
                Debug.LogError($"[UIFramework] Tried to open Screen id {id} but it's not registered as Window or Panel!");
            }
        }

        /// <summary>
        /// 根据传入的ID关闭对应的UI界面，不分面板还是窗口
        /// </summary>
        /// <param name="id"></param>
        public void HideUI(string id)
        {
            if (IsUIRegistered(id, out var type)) {
                if (type == typeof(IWindowController)) {
                    CloseWindow(id);
                }
                else if (type == typeof(IPanelController)) {
                    HidePanel(id);
                }
            }
            else {
                Debug.LogError($"[UIFramework] Tried to open Screen id {id} but it's not registered as Window or Panel!");
            }
        }

        public void RegisterUI(string id, IUIController uiController, Transform uiTransform)
        {
            switch (uiController)
            {
                case IWindowController window when uiTransform:
                    _windowLayer.RegisterUIController(id, window);
                    _windowLayer.ReParentUI(window, uiTransform);
                    break;
                case IPanelController panel when uiTransform:
                    _panelLayer.RegisterUIController(id, panel);
                    _panelLayer.ReParentUI(panel, uiTransform);
                    break;
                default:
                    Debug.LogError("[UIFramework] Transform is null or Unknown uiController");
                    break;
            }
        }

        public void UnregisterUI(string id, IUIController uiController)
        {
            switch (uiController)
            {
                case IWindowController window:
                    _windowLayer.UnregisterUIController(id, window);
                    break;
                case IPanelController panel:
                    _panelLayer.UnregisterUIController(id, panel);
                    break;
                default:
                    Debug.LogError($"[UIFramework] {id} is not registered");
                    break;
            }
        }

        public void HideAllUI(bool animate = true)
        {
            _panelLayer.HideAllUI(animate);
            _windowLayer.HideAllUI(animate);
        }

        public bool IsUIRegistered(string id, out Type type)
        {
            if (_windowLayer.IsRegistered(id))
            {
                type = typeof(IWindowController);
                return true;
            }
            if (_panelLayer.IsRegistered(id))
            {
                type = typeof(IPanelController);
                return true;
            }

            type = null;
            return false;
        }
        
        #endregion
    }
}