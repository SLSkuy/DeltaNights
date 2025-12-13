using System;
using PlayerControl;
using UnityEngine;

namespace GameSetting.GameCamera
{
    /// <summary>
    /// 摄像机设置
    /// </summary>
    [CreateAssetMenu(fileName = "GameCameraSetting", menuName = "GameSetting/GameCamera")]
    public class CameraSetting : ScriptableObject
    {
        /// <summary>
        /// 摄像机灵敏度
        /// </summary>
        [Serializable]
        public struct CameraSensitivity
        {
            [Range(0.1f,10f)]public float horizontal;
            [Range(0.1f,10f)]public float vertical;
        }
        
        [Header("灵敏度设置")]
        [Header("常态灵敏度")]
        public CameraSensitivity normal;
        [Header("肩射状态灵敏度")]
        public CameraSensitivity shoulderAim;
        [Header("开镜状态灵敏度")]
        public CameraSensitivity aim;
        
        [Header("摄像机表现属性设置")] 
        [Tooltip("角色模型旋转阻尼，值越大旋转越慢")]
        public float rotationDamping = 0.2f;
        [Tooltip("常态视角与肩射视角转换过渡时间")]
        public float transitionDuration = 0.15f;
    }
}
