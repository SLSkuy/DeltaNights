/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.13
 *  LastUpdate: 2025.12.13
 * 
 *  功能简述：
 *  CameraManager 用于统一管理游戏中玩家摄像机的状态切换，
 *  基于 Cinemachine 的优先级机制控制当前激活的虚拟摄像机。
 *
 *  主要功能：
 *  - 维护“摄像机状态 → 虚拟摄像机”的映射关系
 *  - 根据游戏状态切换不同摄像机视角
 *  - 通过优先级控制保证同一时间仅一个摄像机生效
 *
 *  使用说明：
 *  - 场景中只允许存在一个 CameraManager 实例（单例）
 *  - 所有虚拟摄像机需在 Inspector 中配置并绑定状态
 *  - 通过 SwitchTo(GameCameraState) 切换摄像机状态
 * ------------------------------------------------------------ */

using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

namespace CameraManage
{
    /// <summary>
    /// 摄像机管理器，管理当前玩家摄像机状态
    /// </summary>
    public class CameraManager : MonoBehaviour
    {
        public static CameraManager Instance { get; private set; }

        /// <summary>
        /// 虚拟摄像机实例
        /// </summary>
        [Serializable]
        public class CameraEntry
        {
            public GameCameraState state;
            public CinemachineCamera camera;
            public int priority;
        }

        [Header("摄像机设置")] 
        [SerializeField] [Tooltip("视角跟踪点，只选择本地玩家的orientation节点")] 
        private Transform orientation;
        
        [SerializeField] private List<CameraEntry> cameras;
        private GameCameraState _currentState;
        private Dictionary<GameCameraState, CameraEntry> _cameraMap;
        
        private void Awake()
        {
            Instance = this;
            
            _cameraMap = new Dictionary<GameCameraState, CameraEntry>();
            foreach (var cam in cameras)
            {
                _cameraMap[cam.state] = cam;
                if(orientation != null) cam.camera.Follow = orientation;    // 设置追踪本地玩家
            }
        }

        public void Start()
        {
            SwitchTo(GameCameraState.Normal);
        }

        /// <summary>
        /// 切换摄像机状态
        /// </summary>
        /// <param name="state">摄像机目标状态</param>
        public void SwitchTo(GameCameraState state)
        {
            if (_currentState == state)
                return;

            _currentState = state;

            foreach (var entry in _cameraMap.Values)
                entry.camera.Priority = 0;

            _cameraMap[state].camera.Priority = _cameraMap[state].priority;
        }
    }
}
