using System;
using System.Collections.Generic;
using Unity.Cinemachine;
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

        [SerializeField] private List<CameraEntry> cameras;
        private GameCameraState _currentState;
        private Dictionary<GameCameraState, CameraEntry> _cameraMap;
        
        private void Awake()
        {
            Instance = this;
            
            _cameraMap = new Dictionary<GameCameraState, CameraEntry>();
            foreach (var cam in cameras)
                _cameraMap[cam.state] = cam;
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
