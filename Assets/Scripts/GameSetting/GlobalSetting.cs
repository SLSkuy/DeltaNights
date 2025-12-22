/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.13
 *  LastUpdate: 2025.12.14
 * 
 *  功能简述：
 *  GlobalSetting 用于集中管理游戏中的全局配置数据，
 *  并在运行时为相关系统提供统一的参数访问入口。
 *
 *  主要功能：
 *  - 管理并分发摄像机相关的全局设置
 *  - 支持使用默认配置或自定义参数覆盖
 *  - 在游戏启动阶段初始化全局参数
 *
 *  使用说明：
 *  - 场景中仅允许存在一个 GlobalSetting 实例
 *  - 可通过 ScriptableObject 作为默认配置来源
 *  - 其他系统通过 GlobalSetting.Instance 读取配置
 * ------------------------------------------------------------ */

using GameSetting.GameCamera;
using Unity.Cinemachine;
using UnityEngine;

namespace GameSetting
{
    /// <summary>
    /// 全局设置
    /// </summary>
    public class GlobalSetting : MonoBehaviour
    {
        public static GlobalSetting Instance { get; private set; }

        [Header("灵敏度设置")]
        public CameraSetting defaultCameraSetting;
        public bool useDefaultCameraSetting;
        [Header("常态灵敏度")]
        public CameraSetting.CameraSensitivity normal;
        [Header("肩射状态灵敏度")]
        public CameraSetting.CameraSensitivity shoulderAim;
        [Header("开镜状态灵敏度")]
        public CameraSetting.CameraSensitivity aim;
        
        [Header("摄像机表现属性设置")] 
        public Camera gameCamera;
        [Tooltip("常态视角与肩射视角转换过渡时间")]
        public float transitionDuration = 0.15f;

        #region 周期函数
        
        private void Awake()
        {
            Instance = this;
            
            if(useDefaultCameraSetting)InitCameraSetting();
        }
        
        #endregion
        
        #region 成员方法

        /// <summary>
        /// 初始化摄像机设置
        /// </summary>
        private void InitCameraSetting()
        {
            // 设置转换过渡时间
            if (gameCamera)
            {
                transitionDuration = defaultCameraSetting.transitionDuration;
                gameCamera.GetComponent<CinemachineBrain>().DefaultBlend.Time = transitionDuration;
            }
            
            normal = defaultCameraSetting.normal;
            shoulderAim = defaultCameraSetting.shoulderAim;
            aim = defaultCameraSetting.aim;
        }
        
        #endregion
    }
}
