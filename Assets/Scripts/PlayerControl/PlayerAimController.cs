/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.11.19
 *  LastUpdate:  2025.12.22
 * 
 *  功能简述：
 *  PlayerAimController 负责玩家视角与角色朝向的联动控制，
 *  根据输入状态（普通 / 肩射 / 开镜）动态调整摄像机灵敏度
 *  与角色模型旋转行为。
 *
 *  主要功能：
 *  - 读取视角输入（ILookInputSource）并驱动摄像机旋转
 *  - 根据瞄准状态切换摄像机模式（Normal / ShoulderAim / Aim）
 *  - 在移动或瞄准时平滑校正玩家角色朝向
 *  - 与 CameraManager 协作完成视角模式切换
 *
 *  设计说明：
 *  - 该类仅负责“视角与角色朝向逻辑”，不处理输入采集
 *  - 通过接口注入输入源，支持本地 / 网络输入复用
 *  - 摄像机灵敏度配置来源于全局配置（GlobalSetting）
 *  - 玩家朝向调整采用阻尼算法，避免突变旋转
 *
 *  使用说明：
 *  - 需挂载在玩家摄像机节点下
 *  - 依赖 PlayerController、GameInput、CameraManager
 *  - 场景中每个玩家仅允许存在一个实例
 * ------------------------------------------------------------ */

using CameraManage;
using GameSetting;
using GameSetting.GameCamera;
using InputProcess;
using Unity.Cinemachine;
using UnityEngine;

namespace PlayerControl
{
    /// <summary>
    /// 玩家视角控制器
    /// </summary>
    public class PlayerAimController : MonoBehaviour
    {
        #region 内部成员
        
        [Header("视角旋转属性")]
        [Tooltip("角色模型旋转阻尼，值越大旋转越慢")]
        [SerializeField]private float rotationDamping = 0.2f;
        
        // 网络同步数据
        public float AimPitch { get; private set; }

        // 组件获取
        private PlayerController _controller;
        private CinemachineInputAxisController _inputAxisController;
        
        // 玩家视角输入
        private ILookInputSource _lookInputSource;
        
        // 控制属性
        private float _curRotationDamping;
        private bool _isAim;
        
        #endregion

        #region 成员方法
        
        /// <summary>
        /// 更新摄像机属性设置
        /// </summary>
        private void UpdateCameraMode(GameCameraState type)
        {
            if (!_inputAxisController) return;

            CameraSetting.CameraSensitivity sens = type switch
            {
                GameCameraState.Normal   => GlobalSetting.Instance.normal,
                GameCameraState.ShoulderAim => GlobalSetting.Instance.shoulderAim,
                GameCameraState.Aim      => GlobalSetting.Instance.aim,
                _ => GlobalSetting.Instance.normal
            };

            foreach (var c in _inputAxisController.Controllers)
            {
                switch (c.Name)
                {
                    case "Horizontal Look":
                        c.Input.Gain = sens.horizontal;
                        break;
                    case "Vertical Look":
                        c.Input.Gain = -sens.vertical;
                        break;
                }
            }

            _curRotationDamping = type == GameCameraState.Normal ? rotationDamping : 0f;
        }
        
        private void UpdatePlayerRotation()
        {
            // 旋转摄像机
            float h = _lookInputSource.HorizontalLook.Value;
            float v = _lookInputSource.VerticalLook.Value;
            transform.localRotation = Quaternion.Euler(v, h, 0);
            
            // Pitch记录（用于网络同步）
            AimPitch = NormalizeAngle(v);
            
            RecenterPlayer(_curRotationDamping);
        }

        /// <summary>
        /// 肩射状态更新
        /// </summary>
        private void UpdateShoulderAimState(bool aimState)
        {
            _isAim = aimState;
            UpdateCameraMode( aimState ? GameCameraState.ShoulderAim : GameCameraState.Normal);
            CameraManager.Instance.SwitchTo(aimState ? GameCameraState.ShoulderAim : GameCameraState.Normal);
        }

        /// <summary>
        /// 开镜瞄准状态更新
        /// </summary>
        private void UpdateAimState(bool aimState)
        {
            _isAim = aimState;
            
            UpdateCameraMode(aimState ? GameCameraState.Aim : GameCameraState.Normal);
            CameraManager.Instance.SwitchTo(aimState ? GameCameraState.Aim : GameCameraState.Normal);
        }
        
        /// <summary>
        /// 重新设置玩家当前朝向
        /// </summary>
        /// <param name="damping">重朝向需要的时间</param>
        private void RecenterPlayer(float damping = 0)
        {
            if (!_controller) return;

            // 没有移动且不处于瞄准状态时，不更新玩家朝向
            if (!_controller.IsMoving() && !_isAim) return;
            
            // 获取玩家模型与当前朝向角度
            var rot = transform.localRotation.eulerAngles;
            rot.y = NormalizeAngle(rot.y);
            var delta = rot.y;
            delta = Damper.Damp(delta, damping, Time.deltaTime);
            
            // 更新玩家模型朝向到当前朝向
            _controller.transform.rotation = Quaternion.AngleAxis(
                delta, _controller.transform.up) *  _controller.transform.rotation;
            
            // 更新朝向角度，避免无限旋转
            _lookInputSource.HorizontalLook.Value -= delta;
            rot.y -= delta;
            transform.localRotation = Quaternion.Euler(rot);
        }
        
        /// <summary>
        /// 限制角度在 -180 ~ 180 范围内
        /// </summary>
        private float NormalizeAngle(float angle)
        {
            while (angle > 180)
                angle -= 360;
            while (angle < -180)
                angle += 360;
            return angle;
        }

        #endregion
        
        #region 周期函数

        private void Awake()
        {
            // 组件引用
            _controller = GetComponentInParent<PlayerController>();
            
            // 初始化赋值
            _curRotationDamping = rotationDamping;
        }

        private void Start()
        {
            // 输入控制注入
            _lookInputSource = GameInput.Instance;
            _inputAxisController = GameInput.Instance.GetComponent<CinemachineInputAxisController>();
            
            // 逻辑注册
            _controller.PostUpdate += UpdatePlayerRotation;
            _controller.OnShoulderAim += UpdateShoulderAimState;
            _controller.OnAim += UpdateAimState;
            
            // 更新摄像机设置
            UpdateCameraMode(GameCameraState.Normal);
        }

        private void OnEnable()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void OnDisable()
        {
            Cursor.lockState = CursorLockMode.None;
        }

        private void OnDestroy()
        {
            _controller.PostUpdate -= UpdatePlayerRotation;
            _controller.OnShoulderAim -= UpdateShoulderAimState;
            _controller.OnAim -= UpdateAimState;
        }
        
        #endregion
    }
}