using System.Collections.Generic;
using UnityEngine;

namespace UIFramework.Core
{
    /// <summary>
    /// 基础UI界面的Layer
    /// 每个Layer层的子对象为需要管理的UI对象
    /// 通过访问管理对象的controller进行UI界面的控制
    /// </summary>
    /// <typeparam name="T">UI界面控制器类型</typeparam>
    public abstract class UIBaseLayer<T> : MonoBehaviour where T : IUIController
    {
        #region 内部成员

        /// <summary>
        /// 当前层所管理的所有UI界面控制器
        /// </summary>
        protected Dictionary<string, T> RegisteredUIControllers;

        #endregion

        #region UI界面控制器管理方法

        /// <summary>
        /// 对外显示UI接口，由子类实现
        /// </summary>
        /// <param name="controller"></param>
        public abstract void ShowUI(T controller);
        
        /// <summary>
        /// 对外带参数显示UI接口，由子类实现
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="props"></param>
        /// <typeparam name="TProps"></typeparam>
        public abstract void ShowUI<TProps>(T controller, TProps props) where TProps : IUIProperties;
        
        /// <summary>
        /// 对外隐藏UI接口，由子类实现
        /// </summary>
        /// <param name="controller"></param>
        public abstract void HideUI(T controller);

        /// <summary>
        /// 通过ID显示UI界面
        /// </summary>
        /// <param name="id">UI界面ID</param>
        public void ShowUIByID(string id)
        {
            if (RegisteredUIControllers.TryGetValue(id, out T controller))
            {
                ShowUI(controller);
            }
            else
            {
                Debug.LogWarning($"[{GetType()}] UI controller with ID {id} not found");
            }
        }

        /// <summary>
        /// 带属性的通过ID显示UI界面
        /// </summary>
        /// <param name="id">UI界面ID</param>
        /// <param name="props">UI界面属性参数</param>
        /// <typeparam name="TProps">UI界面属性类型</typeparam>
        public void ShowUIByID<TProps>(string id, TProps props) where TProps : IUIProperties
        {
            if (RegisteredUIControllers.TryGetValue(id, out T controller))
            {
                ShowUI<TProps>(controller, props);
            }
            else
            {
                Debug.LogWarning($"[{GetType()}] UI controller with ID {id} not found");
            }
        }

        /// <summary>
        /// 通过ID隐藏UI界面
        /// </summary>
        /// <param name="id"></param>
        public void HideUIByID(string id)
        {
            if (RegisteredUIControllers.TryGetValue(id, out T controller))
            {
                HideUI(controller);
            }
            else
            {
                Debug.LogWarning($"[{GetType()}] UI controller with ID {id} not found");
            }
        }

        /// <summary>
        /// 隐藏所有UI界面
        /// </summary>
        /// <param name="isAnimate">是否播放动画</param>
        public virtual void HideAllUI(bool isAnimate = true)
        {
            foreach (T controller in RegisteredUIControllers.Values)
            {
                controller.Hide(isAnimate);
            }
        }
        
        #endregion

        #region UI层管理方法

        /// <summary>
        /// 初始化当前管理层
        /// </summary>
        public virtual void Initialize()
        {
            RegisteredUIControllers = new Dictionary<string, T>();
        }

        /// <summary>
        /// 使UI界面作为当前Layer的子对象
        /// 在Inspector窗口中分类
        /// </summary>
        /// <param name="controller">UI界面控制器</param>
        /// <param name="uiTransform">UI界面Transform</param>
        public virtual void ReParentUI(IUIController controller, Transform uiTransform)
        {
            uiTransform.SetParent(this.transform);
        }

        /// <summary>
        /// 在当前Layer注册指定ID的UI界面
        /// 封装注册逻辑
        /// </summary>
        /// <param name="uiControllerID">UI界面ID</param>
        /// <param name="controller">UI界面控制器参数</param>
        public void RegisterUIController(string uiControllerID, T controller)
        {
            if (!IsRegistered(uiControllerID))
            {
                ProcessUIRegister(uiControllerID, controller);
            }
            else
            {
                Debug.LogError("[UIBaseLayer] UI Controller already registered for id: " + uiControllerID);
            }
        }

        /// <summary>
        /// 在当前Layer注销指定ID的UI界面
        /// 封装注销逻辑
        /// </summary>
        /// <param name="uiControllerID">UI界面ID</param>
        /// <param name="controller">UI界面控制器参数</param>
        public void UnregisterUIController(string uiControllerID, T controller)
        {
            if (RegisteredUIControllers.ContainsKey(uiControllerID))
            {
                ProcessUIUnregister(uiControllerID, controller);
            }
            else
            {
                Debug.LogError("[UIBaseLayer] UI Controller doesn't exist for id: " + uiControllerID);
            }
        }

        /// <summary>
        /// UI注册逻辑
        /// </summary>
        /// <param name="uiControllerID">UI界面ID</param>
        /// <param name="controller">UI界面控制器参数</param>
        protected virtual void ProcessUIRegister(string uiControllerID, T controller)
        {
            controller.UIControllerID = uiControllerID;
            RegisteredUIControllers.Add(uiControllerID, controller);
            controller.UIDestroyed += OnUIDestroyed;
        }

        /// <summary>
        /// UI注销逻辑
        /// </summary>
        /// <param name="uiControllerID">UI界面ID</param>
        /// <param name="controller">UI界面控制器参数</param>
        protected virtual void ProcessUIUnregister(string uiControllerID, T controller)
        {
            RegisteredUIControllers.Remove(uiControllerID);
            controller.UIDestroyed -= OnUIDestroyed;
        }

        /// <summary>
        /// 检测对应ID的UI界面是否在当前Layer注册
        /// </summary>
        /// <param name="uiControllerID">UI界面ID</param>
        public bool IsRegistered(string uiControllerID)
        {
            return RegisteredUIControllers.ContainsKey(uiControllerID);
        }

        /// <summary>
        /// UI界面销毁时调用
        /// </summary>
        /// <param name="controller">UI界面控制器参数</param>
        private void OnUIDestroyed(IUIController controller)
        {
            if (!string.IsNullOrEmpty(controller.UIControllerID))
            {
                UnregisterUIController(controller.UIControllerID, (T) controller);
            }
            else
            {
                Debug.LogWarning("[UIBaseLayer] UI Controller's ID is null or empty");
            }
        }

        #endregion
    }
}