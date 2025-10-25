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
        
        [SerializeField] [Tooltip("是否自动初始化UI框架")]
        private bool initializeOnAwake = true;
        
        // UI类别层级管理器
        private PanelLayer _panelLayer;
        private WindowLayer _windowLayer;
        
        private Canvas _mainCanvas;
        private GraphicRaycaster _graphicRaycaster;
        
        #endregion
        
        #region 暴露属性

        public static UIFramework Instance { get; private set; }
        public UIFrameworkSettings uiSettings;
        public Canvas MainCanvas { get { if (!_mainCanvas)_mainCanvas = _mainCanvas.GetComponent<Canvas>(); return _mainCanvas; } }
        public Camera CanvasCamera => _mainCanvas.worldCamera;

        #endregion
        
        private void Awake()
        {
            if (initializeOnAwake)
            {
                Initialize();
                RegisterAllUIPrefab();
            }
            
            ShowUI("TestWindow");
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                ShowUI("TestWindow");
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                ShowUI("PopupWindow");
            }else if (Input.GetKeyDown(KeyCode.E))
            {
                ShowUI("EnqueueWindow");
            }
        }

        private void Initialize()
        {
            if(Instance == null)Instance = this;
            
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
        }

        private void RegisterAllUIPrefab()
        {
            foreach (var obj in uiSettings.uiToRegister)
            {
                GameObject prefab = Instantiate(obj);
                IUIController controller = prefab.GetComponent<IUIController>();
                if(!uiSettings.uiToEnableAtStart.Contains(controller.UIControllerID))prefab.SetActive(false);
                RegisterUI(controller.UIControllerID, controller,prefab.transform);
            }
        }

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

        public void HidePanel(string id)
        {
            _panelLayer.HideUIByID(id);
        }

        public void OpenWindow(string id)
        {
            _windowLayer.ShowUIByID(id);
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
        /// <param name="id"></param>
        public void ShowUI(string id)
        {
            Type type;
            if (IsUIRegistered(id, out type)) {
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

        public void HideAll(bool animate = true)
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
    }
}