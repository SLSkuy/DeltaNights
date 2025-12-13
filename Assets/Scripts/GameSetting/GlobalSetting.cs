using System;
using GameSetting.GameCamera;
using UnityEngine;

namespace GameSetting
{
    public class GlobalSetting : MonoBehaviour
    {
        public static GlobalSetting Instance { get; private set; }

        [Header("灵敏度设置")]
        public CameraSetting defaultCameraSetting;
        public bool useDefaultCameraSetting;
        [Range(1f,10f)]public float horizontalSensitive = 1f;
        [Range(1f,10f)]public float verticalSensitive = 1f;
        
        [Header("摄像机控制表现属性")] 
        [Tooltip("角色模型旋转阻尼，值越大旋转越慢")]
        public float rotationDamping = 0.2f;
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
            rotationDamping = defaultCameraSetting.rotationDamping;
            horizontalSensitive = defaultCameraSetting.horizontalSensitive;
            verticalSensitive = defaultCameraSetting.verticalSensitive;
        }
        
        #endregion
    }
}
