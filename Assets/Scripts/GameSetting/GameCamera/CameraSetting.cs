/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.13
 *  LastUpdate: 2025.12.14
 * 
 *  功能简述：
 *  CameraSetting 为摄像机系统的配置数据类，
 *  用于集中管理不同摄像机状态下的灵敏度与表现参数。
 *
 *  主要功能：
 *  - 配置常态、肩射、开镜等状态的摄像机灵敏度
 *  - 提供摄像机状态切换时的过渡表现参数
 *
 *  使用说明：
 *  - 通过 ScriptableObject 在编辑器中创建与维护
 *  - 不包含摄像机控制逻辑，仅作为参数数据源
 *  - 由摄像机控制或管理模块读取并应用配置
 * ------------------------------------------------------------ */

using System;
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
        [Tooltip("常态视角与肩射视角转换过渡时间")]
        public float transitionDuration = 0.15f;
    }
}
