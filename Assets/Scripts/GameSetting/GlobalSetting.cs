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

        #region 声明周期函数
        
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
