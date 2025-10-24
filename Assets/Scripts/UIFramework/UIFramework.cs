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
        
        public Canvas MainCanvas { get { if (!_mainCanvas)_mainCanvas = _mainCanvas.GetComponent<Canvas>(); return _mainCanvas; } }
        public Camera CanvasCamera => _mainCanvas.worldCamera;

        #endregion
        
        private void Awake()
        {
            if (initializeOnAwake)
                Initialize();
        }

        private void Initialize()
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
        }

        private void BlockScreen()
        {
            _graphicRaycaster.enabled = false;
        }

        private void UnblockScreen()
        {
            _graphicRaycaster.enabled = true;
        }
    }
}