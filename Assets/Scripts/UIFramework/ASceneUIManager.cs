using System;
using UnityEngine;

namespace UIFramework
{
    /// <summary>
    /// 场景关卡UI管理基类，不同场景的UI管理器都继承自该类
    /// 该类用于管理所有UI事件回调
    /// </summary>
    [RequireComponent(typeof(UIFramework))]
    public class ASceneUIManager : MonoBehaviour
    {
        [Header("UI To Register List")]
        [SerializeField] [Tooltip("所有需要注册到该场景的UI列表")]
        private UIFrameworkSettings uiSettings;
        protected UIFramework UIFrame;
        
        #region 暴露属性

        public UIFrameworkSettings UISettings { get => uiSettings; set => uiSettings = value; }

        #endregion
        
        private void Awake()
        {
            UIFrame = GetComponent<UIFramework>();
            UIFrame.UISettings = uiSettings;
            UIFrame.Initialize();
            
            AddSignal();
        }

        private void OnDestroy()
        {
            RemoveSignal();
        }

        protected virtual void AddSignal()
        {
            // 添加到事件系统中
        }

        protected virtual void RemoveSignal()
        {
            // 从事件系统中系统
        }
    }
}