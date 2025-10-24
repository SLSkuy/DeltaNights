using System;
using UnityEngine;

namespace UIFramework.UIAnimation
{
    /// <summary>
    /// UI界面动画组件，用于设置UI界面的过渡行为
    /// 继承该类并实现Animate方法用于实现UI界面动画的自定义
    /// 该类为动画处理器，需要单独挂载到场景上
    /// </summary>
    [Serializable]
    public abstract class AnimComponent : MonoBehaviour
    {
        public abstract void Animate(Transform target, Action callback);
    }
}