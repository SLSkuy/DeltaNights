using System;
using System.Collections.Generic;
using PlayerControl.PlayerFSM;
using Unity.Cinemachine;
using UnityEngine;

namespace PlayerControl
{
    /// <summary>
    /// 玩家移动控制器
    /// </summary>
    public class PlayerController : MonoBehaviour, IInputAxisOwner
    {
        #region 内部成员

        [Header("玩家属性（Debug）")] 
        public float speed = 1f;
        public float sprintSpeed = 4f;
        public float jumpSpeed = 4f;
        public float sprintJumpSpeed = 6f;
        public float damping = 0.5f;
        public bool Grounded => IsGrounded();

        [Header("输入轴配置")]
        [Tooltip("X轴移动 范围(-1,1) 控制左右移动")]
        public InputAxis moveX = InputAxis.DefaultMomentary;
        [Tooltip("Y轴移动 范围(-1,1) 控制前后移动")]
        public InputAxis moveZ = InputAxis.DefaultMomentary;
        [Tooltip("跳跃 值为0或1 控制垂直移动")]
        public InputAxis jump = InputAxis.DefaultMomentary;
        [Tooltip("冲刺 值为0或1 控制冲刺状态")]
        public InputAxis sprint = InputAxis.DefaultMomentary;
        
        [Header("物理属性")]
        [Tooltip("地面层，用于检测是否接触到地面")]
        public LayerMask groundLayers;
        [Tooltip("玩家重力属性")] // 自行实现物理效果，不使用Unity自带的RigidBody，减少性能开销
        public float gravity = 9.8f;
        
        // 组件获取
        private PlayerFiniteStateMachine _finiteStateMachine;
        private CharacterController _characterController;
        private Camera _camera;
        
        // 预输入处理
        private const float KeyDelayBeforeInferringJump = 0.3f;     // 按下跳跃键后在多少延迟内认定为能够执行跳跃操作
        private float _timeLastGrounded;
        
        // 信息存储
        private Vector3 _lastInput;
        private Vector3 _currentVelocityXZ; // 后面以根动画速度代替
        private float _currentVelocityY;
        private bool _isSprinting;
        private bool _isJumping;

        #endregion
        
        #region 事件

        public event Action PreUpdate;  // 每帧更新前调用
        public event Action PostUpdate; // 每帧更新后调用 
        
        #endregion
        
        #region cinemachine输入控制
        
        /// <summary>
        /// 实现IInputAxisOwner接口
        /// 用于Cinemachine Input Axis Controller读取相应信息以获取Input System中的输入信息
        /// </summary>
        /// <param name="axes"></param>
        public void GetInputAxes(List<IInputAxisOwner.AxisDescriptor> axes)
        {
            axes.Add(new () { DrivenAxis = () => ref moveX, Name = "Move X", Hint = IInputAxisOwner.AxisDescriptor.Hints.X });
            axes.Add(new () { DrivenAxis = () => ref moveZ, Name = "Move Z", Hint = IInputAxisOwner.AxisDescriptor.Hints.Y });
            axes.Add(new () { DrivenAxis = () => ref jump, Name = "Jump" });
            axes.Add(new () { DrivenAxis = () => ref sprint, Name = "Sprint" });
        }
        
        /// <summary>
        /// 编辑器更新时限定填入值在规定范围内
        /// </summary>
        private void OnValidate()
        {
            moveX.Validate();
            moveZ.Validate();
            jump.Validate();
            sprint.Validate();
        }
        
        #endregion
        
        #region 成员方法

        /// <summary>
        /// 检测玩家当前是否在移动
        /// </summary>
        public bool IsMoving()
        {
            float movingThreshold = 0.001f;
            return _lastInput.sqrMagnitude > movingThreshold;
        }

        /// <summary>
        /// 检测玩家当前是否触地
        /// </summary>
        public bool IsGrounded()
        {
            const float distanceFromGroundThreshold = 10f;
            const float groundedThreshold = 0.01f;
            return GetDistanceFromGround(transform.position, distanceFromGroundThreshold) < groundedThreshold;
        }

        /// <summary>
        /// 检测玩家当前与地面的距离
        /// </summary>
        /// <param name="pos">当前玩家位置</param>
        /// <param name="max">射线检测距离</param>
        /// <returns></returns>
        private float GetDistanceFromGround(Vector3 pos, float max)
        {
            // 忽略Trigger
            if (Physics.Raycast(pos, Vector3.down, out RaycastHit hit,
                    max, groundLayers, QueryTriggerInteraction.Ignore))
            {
                return hit.distance;
            }
            return max + 1; // 未检测到地面，返回 max + 1 以确保判断当前为未触地状态
        }

        /// <summary>
        /// 计算当前速度朝向
        /// </summary>
        private void CalculateCurrentVelocity()
        {
            float x = moveX.Value;
            float z = moveZ.Value;

            // 根据摄像机朝向投影到水平面
            Vector3 camForward = Vector3.ProjectOnPlane(_camera.transform.forward, Vector3.up).normalized;
            Vector3 camRight = Vector3.ProjectOnPlane(_camera.transform.right, Vector3.up).normalized;

            // 计算移动方向
            Vector3 desiredDir = camForward * z + camRight * x;
            _lastInput = desiredDir;

            if (!_isJumping)
            {
                // 判断是否在奔跑状态
                _isSprinting = sprint.Value > 0.5f;
                
                Vector3 desiredVelocity = _lastInput * (_isSprinting ? sprintSpeed : speed);
                _currentVelocityXZ += Damper.Damp(desiredVelocity - _currentVelocityXZ,damping, Time.deltaTime);    
            }
        }

        /// <summary>
        /// 应用玩家位移
        /// </summary>
        private void ApplyMotion()
        {
            if (_characterController)
            {
                // _characterController.Move((_currentVelocityY * Vector3.up + _currentVelocityXZ) * Time.deltaTime);
                _characterController.SimpleMove(_currentVelocityXZ);
            }
        }
        
        /// <summary>
        /// 传送玩家到指定位置并重新设置当前朝向
        /// </summary>
        /// <param name="newPos">传送目标位置</param>
        /// <param name="newRot">目标朝向</param>
        public void Teleport(Vector3 newPos, Quaternion newRot)
        {
            // 关闭玩家控制器，防止强制更新导致位置变换失效
            if(_characterController != null)
                _characterController.enabled = false;
            
            Quaternion rot = transform.rotation;
            Quaternion deltaRot = rot * Quaternion.Inverse(rot);
            _currentVelocityXZ = deltaRot * _currentVelocityXZ;     // 设置移动朝向
            transform.SetPositionAndRotation(newPos, deltaRot);
            
            if(_characterController != null)
                _characterController.enabled = true;
        }
        
        #endregion
        
        #region 周期函数
        
        private void Awake()
        {
            // 初始化内部成员
            _finiteStateMachine = new PlayerFiniteStateMachine();
            
            // 组件引用
            _characterController = GetComponent<CharacterController>();
            _camera = Camera.main;
        }

        private void Start()
        {
            // 逻辑注册
            _finiteStateMachine.SwitchState(PlayerState.Idle);
        }
        
        private void OnEnable()
        {
            _currentVelocityXZ = Vector3.zero;
            _currentVelocityY = 0;
            _isSprinting = false;
            _isJumping = false;
        }

        private void Update()
        {
            PreUpdate?.Invoke();
            
            CalculateCurrentVelocity();
            
            _finiteStateMachine.Update();
            
            // 测试移动方法，后续移动到状态内部实现
            ApplyMotion();
        }

        private void LateUpdate()
        {
            _finiteStateMachine.LateUpdate();
            
            // 更新摄像机
            PostUpdate?.Invoke();
        }

        private void FixedUpdate() => _finiteStateMachine.FixedUpdate();
        private void OnAnimatorMove() => _finiteStateMachine.OnAnimatorMove();
        
        #endregion
    }
}