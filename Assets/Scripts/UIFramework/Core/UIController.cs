using System;
using UIFramework.UIAnimation;
using Unity.VisualScripting;
using UnityEngine;

namespace UIFramework.Core
{
    /// <summary>
    /// UI界面控制器基类，用于实现单个UI界面的所有逻辑
    /// </summary>
    /// <typeparam name="T">UI界面属性类型</typeparam>
    public class UIController<T> : MonoBehaviour, IUIController where T : IUIProperties
    {
        #region 控制器属性

        [Header("UI过渡动画")] 
        [SerializeField] [Tooltip("显示动画")] private AnimComponent animIn;
        [SerializeField] [Tooltip("隐藏动画")] private AnimComponent animOut;
        
        [Header("UI属性")]
        [SerializeField] private string uiControllerID;
        [SerializeField] private bool isVisible;
        [SerializeField] [Tooltip("UI界面属性，用于控制多UI界面时的处理逻辑，可为空")] 
        private T properties;

        public event Action<IUIController> UIDestroyed;
        public event Action<IUIController> InTransitionFinished;
        public event Action<IUIController> OutTransitionFinished;
        public event Action<IUIController> CloseRequested;
        
        #endregion

        #region 暴露属性
        
        public AnimComponent AnimIn { get => animIn; set => animIn = value; }
        public AnimComponent AnimOut { get => animOut; set => animOut = value; }
        public string UIControllerID { get => uiControllerID; set => uiControllerID = value; }
        public bool IsVisible { get => isVisible; set => isVisible = value; }
        public T Properties { get => properties; set => properties = value; }
        
        #endregion

        #region 控制器方法
        
        private void Awake()
        {
            AddListener();
        }

        private void OnDestroy()
        {
            UIDestroyed?.Invoke(this);
            RemoveListener();
        }
        
        /// <summary>
        /// 显示界面
        /// </summary>
        /// <param name="props">界面属性</param>
        public void Show(IUIProperties props = null)
        {
            if (props != null)
            {
                if (props is T uiProperties)
                {
                    SetProperties(uiProperties);
                }
                else
                {
                    Debug.LogError($"{GetType()} wrong property type {props.GetType().Name}");
                    return;
                }
            }
            
            HierarchyFixOnShow();
            OnPropertiesChanged();

            if (!gameObject.activeSelf)
            {
                DoAnimation(animIn, OnTransitionInFinished, true);
            }
            else
            {
                InTransitionFinished?.Invoke(this);
            }
        }

        /// <summary>
        /// 隐藏界面
        /// </summary>
        /// <param name="animate">是否播放动画</param>
        public void Hide(bool animate = true)
        {
            DoAnimation(animate ? animOut : null, OnTransitionOutFinished, false);
            WhileHiding();
        }

        private void DoAnimation(AnimComponent anim, Action callback, bool visible)
        {
            if (anim == null)
            {
                // 没有过渡动画处理
                gameObject.SetActive(visible);
                callback?.Invoke();
            }
            else
            {
                // 当前UI界面是否显示
                if (visible && !gameObject.activeSelf)
                {
                    gameObject.SetActive(true);
                }
                
                anim.Animate(transform, callback);
            }
        }

        private void OnTransitionInFinished()
        {
            isVisible = true;
            InTransitionFinished?.Invoke(this);
        }

        private void OnTransitionOutFinished()
        {
            isVisible = false;
            InTransitionFinished?.Invoke(this);
        }

        /// <summary>
        /// 添加监听事件
        /// </summary>
        protected virtual void AddListener()
        {
            
        }

        /// <summary>
        /// 注销监听事件
        /// </summary>
        protected virtual void RemoveListener()
        {
            UIDestroyed = null;
            InTransitionFinished = null;
            OutTransitionFinished = null;
            CloseRequested = null;
        }

        /// <summary>
        /// 属性参数设置到界面后的时候触发
        /// </summary>
        protected virtual void OnPropertiesChanged()
        {
            
        }

        /// <summary>
        /// 界面隐藏时触发处理逻辑
        /// </summary>
        protected virtual void WhileHiding()
        {
            
        }

        protected virtual void SetProperties(T props)
        {
            properties = props;
            
        }

        /// <summary>
        /// 在显示的时候处理一些层级，或者属性处理等
        /// </summary>
        protected virtual void HierarchyFixOnShow()
        {
            
        }
        
        #endregion
    }
}