using UIFramework.Panel;
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
        
        private Canvas _mainCanvas;
        private GraphicRaycaster _graphicRaycaster;
        
        #endregion
        
        #region 暴露属性
        
        public Canvas MainCanvas { get { if (!_mainCanvas)_mainCanvas = _mainCanvas.GetComponent<Canvas>(); return _mainCanvas; } }
        public Camera CanvasCamera { get => _mainCanvas.worldCamera; }

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
        }
    }
}