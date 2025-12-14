using System;
using InputProcess;
using PlayerControl.PlayerFSM;
using Unity.Cinemachine;
using UnityEngine;

namespace PlayerControl
{
    /// <summary>
    /// 玩家移动控制器
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        #region 内部成员

        [Header("玩家移动属性")] 
        public float speed = 4f;
        public float shoulderAimSpeed = 2.5f;
        public float aimSpeed = 1.5f;
        public float jumpSpeed = 4f;
        public float locomotionDamping = 0.5f;
        public bool Grounded => IsGrounded();
        
        [Header("玩家跳跃属性")]
        public int maxJumpCount = 2;
        public float airMoveFactor = 0.8f;
        
        [Header("物理属性")]
        [Tooltip("地面层，用于检测是否接触到地面")]
        public LayerMask groundLayers;
        public float groundThreshold = 0.1f;
        [Tooltip("玩家重力属性")] // 自行实现物理效果，不使用Unity自带的RigidBody，减少性能开销
        public float gravity = 9.8f;
        
        // 组件获取
        private PlayerFiniteStateMachine _finiteStateMachine;
        private CharacterController _characterController;
        private Camera _camera;
        
        // 玩家移动输入
        private ILocomotionInputSource _locomotionInputSource;
        
        // 移动属性
        private Vector3 _lastInput;
        private Vector3 _currentVelocityXZ; // 后面以根动画速度代替
        private float _currentVelocityY;
        
        // 跳跃属性
        private bool _isJumping;
        private int _jumpCount;
        private bool _jumpPressedLastFrame;
        private float _timeLastGrounded;
        
        // 瞄准属性
        private bool _isShoulderAim;
        private bool _aimPressedLastFrame;  // 切换型状态
        private bool _isAim;

        #endregion
        
        #region 事件

        public event Action PreUpdate;  // 每帧更新前调用
        public event Action PostUpdate; // 每帧更新后调用 
        public event Action<bool> OnShoulderAim; // 是否为肩射状态
        public event Action<bool> OnAim;  // 是否为开镜瞄准状态
        public event Action<bool> ShowMesh; // 是否显示模型
        public event Action<int> OnJump;    // 玩家跳跃事件
        public event Action OnLand; // 触地事件
        
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
            return GetDistanceFromGround(transform.position, distanceFromGroundThreshold) < groundThreshold;
        }

        /// <summary>
        /// 检测玩家当前与地面的距离
        /// </summary>
        /// <param name="pos">当前玩家位置</param>
        /// <param name="max">射线检测距离</param>
        /// <returns>离地面距离</returns>
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
            float x = _locomotionInputSource.MoveX;
            float z = _locomotionInputSource.MoveZ;

            // 根据摄像机朝向投影到水平面
            Vector3 camForward = Vector3.ProjectOnPlane(_camera.transform.forward, Vector3.up).normalized;
            Vector3 camRight = Vector3.ProjectOnPlane(_camera.transform.right, Vector3.up).normalized;

            // 计算移动方向
            Vector3 desiredDir = camForward * z + camRight * x;
            _lastInput = desiredDir;

            Vector3 desiredVelocity = _lastInput * (_isAim ? aimSpeed : _isShoulderAim ? shoulderAimSpeed : speed);
            if (_isJumping) desiredVelocity *= airMoveFactor;   // 空中移动降低水平位移效果
            _currentVelocityXZ += Damper.Damp(desiredVelocity - _currentVelocityXZ,locomotionDamping, Time.deltaTime); 
        }

        /// <summary>
        /// 应用玩家位移
        /// </summary>
        private void ApplyMotion()
        {
            if (_characterController) { 
                // _characterController.Move((_currentVelocityY * Vector3.up + _currentVelocityXZ) * Time.deltaTime);
                // _characterController.SimpleMove(_currentVelocityXZ); 
                
                Vector3 motion = _currentVelocityXZ + Vector3.up * _currentVelocityY;
                _characterController.Move(motion * Time.deltaTime);
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

        /// <summary>
        /// 检测瞄准状态
        /// </summary>
        private void UpdateAimState()
        {
            // 检测是否为肩射状态
            bool shoulderNow = _locomotionInputSource.ShoulderAim > 0.1f;
            if (_isShoulderAim != shoulderNow)
            {
                _isShoulderAim = shoulderNow;
                _isAim = false;
                OnShoulderAim?.Invoke(_isShoulderAim);
                ShowMesh?.Invoke(true); // 确保从开镜状态转换模型也能正常显示
            }

            // 检测是否为开镜状态
            bool aimPressedNow = _locomotionInputSource.Aim > 0.1f;
            if (aimPressedNow && !_aimPressedLastFrame)
            {
                _isAim = !_isAim;
                OnAim?.Invoke(_isAim);
                ShowMesh?.Invoke(!_isAim);  // 根据开镜状态显示模型
            }
            _aimPressedLastFrame = aimPressedNow;
        }

        /// <summary>
        /// 检测跳跃输入
        /// </summary>
        private void UpdateJumpState()
        {
            bool jumpPressedNow = _locomotionInputSource.Jump > 0.1f;

            // 边沿触发
            if (jumpPressedNow && !_jumpPressedLastFrame)
            {
                TryJump();
            }

            _jumpPressedLastFrame = jumpPressedNow;
        }
        
        /// <summary>
        /// 判断是否能够跳跃
        /// </summary>
        private void TryJump()
        {
            if (_jumpCount >= maxJumpCount)
                return;

            _jumpCount++;
            _isJumping = true;

            // 给予向上的初速度
            _currentVelocityY = jumpSpeed;
            
            OnJump?.Invoke(_jumpCount);
        }
        
        /// <summary>
        /// 自行实现重力，用于实现不同角色的特殊技能
        /// </summary>
        private void ApplyGravity()
        {
            if (Grounded)
            {
                if (_currentVelocityY < 0)
                {
                    // 以一定的下落速度触地时触发落地事件
                    if(_currentVelocityY < -3f)OnLand?.Invoke();
                    
                    _currentVelocityY = -0.1f; // 贴地（防止抖动）
                    _isJumping = false;
                    _jumpCount = 0;         // 落地 → 重置跳跃次数
                }
            }
            else
            {
                if(_jumpCount < 1)_jumpCount = 1;   // 从边缘坠落时只允许跳跃一次
                _currentVelocityY -= gravity * Time.deltaTime;
            }
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
            // 输入注入
            _locomotionInputSource = GameInput.Instance;
            
            // 逻辑注册
            _finiteStateMachine.SwitchState(PlayerState.Idle);
        }
        
        private void OnEnable()
        {
            _currentVelocityXZ = Vector3.zero;
            _currentVelocityY = 0;
            _isJumping = false;
            _isShoulderAim = false;
        }

        private void Update()
        {
            PreUpdate?.Invoke();
            
            UpdateJumpState();
            CalculateCurrentVelocity();
            ApplyGravity(); // 计算重力
            
            _finiteStateMachine.Update();
            
            // 测试移动方法，后续移动到状态内部实现
            ApplyMotion();
        }

        private void LateUpdate()
        {
            _finiteStateMachine.LateUpdate();
            
            // 检测摄像机状态更新
            UpdateAimState();
            
            // 更新摄像机
            PostUpdate?.Invoke();
        }

        private void FixedUpdate() => _finiteStateMachine.FixedUpdate();
        private void OnAnimatorMove() => _finiteStateMachine.OnAnimatorMove();
        
        #endregion
    }
}