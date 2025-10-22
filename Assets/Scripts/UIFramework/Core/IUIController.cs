using System;

namespace UIFramework.Core
{
    /// <summary>
    /// UI界面控制器接口
    /// 声明所有UI界面控制器应有属性及方法
    /// 该类则为UI界面的逻辑层
    /// </summary>
    public interface IUIController
    {
        #region 控制器属性
        
        /// <summary>
        /// 每个UI界面控制器的唯一ID
        /// </summary>
        string UIControllerID { get; set; }
        
        #endregion
        
        #region 基础操作
        
        void Show(IUIProperties properties = null);
        void Hide(bool animate = true);
        
        #endregion

        #region 回调函数

        event Action<IUIController> OnTransitionInFinished;
        event Action<IUIController> OnTransitionOutFinished;

        #endregion
    }
}