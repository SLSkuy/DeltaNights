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
        
    }
}