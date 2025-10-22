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
        private Dictionary<string, T> registeredUIControllers;

        #endregion

        #region UI界面控制器方法

        // 基类控制器方法，由子类具体实现UI界面显示逻辑
        public abstract void Show(T controller);
        public abstract void Show<TProps>(T controller, TProps props) where TProps : IUIProperties;
        public abstract void Hide(T controller);
        
        #endregion

        #region UI层管理方法

        public virtual void RegisterUIController(string uiControllerID, T controller)
        {
        }

        public virtual void UnregisterUIController(string uiControllerID)
        {
        }

        #endregion
    }
}