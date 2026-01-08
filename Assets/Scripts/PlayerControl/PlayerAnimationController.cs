/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.15
 *  LastUpdate:  2025.12.19
 * 
 *  功能简述：
 *  PlayerAnimationController 负责玩家角色动画状态的驱动与平滑过渡，
 *  将 PlayerController 中的行为事件映射为 Animator 参数与动画层权重变化。
 *
 *  主要功能：
 *  - 根据移动输入更新移动动画参数（MoveX / MoveZ）
 *  - 监听跳跃事件并触发跳跃动画
 *  - 根据肩射状态平滑切换动画层级权重
 *  - 通过插值方式避免动画参数突变
 *
 *  设计说明：
 *  - 动画控制采用“事件驱动”而非轮询输入，降低系统耦合
 *  - 动画参数更新与动画层权重更新相互独立，便于扩展
 *  - 不直接参与角色逻辑计算，仅负责 Animator 参数映射
 *  - 肩射动画通过独立动画层实现，保证基础移动动画可复用
 *
 *  使用说明：
 *  - 需与 PlayerController、Animator 组件挂载在同一对象上
 *  - Animator 中需存在对应参数：
 *      - Float：MoveX、MoveZ
 *      - Trigger：Jump
 *  - shoulderAimLayerIndex 对应 Animator 中的肩射动画层
 * ------------------------------------------------------------ */

using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace PlayerControl
{
    /// <summary>
    /// 玩家动画控制器
    /// </summary>
    public class PlayerAnimationController : MonoBehaviour
    {
        [Header("动画过渡速度")]
        [SerializeField] private float moveAnimSmooth = 10f;

        [Header("层级过渡速度")] 
        [SerializeField] private int shoulderAimLayerIndex = 1;
        [SerializeField] private float shoulderAimLayerSmooth = 10f;
        
        // 组件引用
        private PlayerController _controller;
        private PlayerAttackController _attackController;
        private Animator _animator;

        // 肩射动画层级
        private float _currentShoulderLayerWeight;
        private float _targetShoulderLayerWeight;
        
        // 输入属性
        private Vector2 _currentMoveInput;
        private Vector2 _targetMoveInput;
        private int _moveZHash;
        private int _moveXHash;
        
        // 跳跃哈希
        private int _jumpHash;

        //动画rig
        public Rig _rig;

        private bool _isReloading = false;

        #region 周期函数
        private void Awake()
        {
            _controller = GetComponent<PlayerController>();
            _attackController = GetComponent<PlayerAttackController>();
            _animator = GetComponent<Animator>();
            _rig = GetComponentInChildren<Rig>();
            
            _moveZHash = Animator.StringToHash("MoveZ");
            _moveXHash = Animator.StringToHash("MoveX");
            _jumpHash = Animator.StringToHash("Jump");
        }

        private void Start()
        {
            _controller.OnMove += SetAnimMoveInputTarget;
            _controller.OnJump += SetAnimJump;
            _controller.OnShoulderAim += SetShoulderAimState;
            _controller.OnLand += SetAnimLand;

            _attackController.OnIdle += SetIdle;
            _attackController.OnAim += SetAim;
            _attackController.OnReloadStart += SetReload;

        }


        private void OnDestroy()
        {
            _controller.OnMove -= SetAnimMoveInputTarget;
            _controller.OnJump -= SetAnimJump;
            _controller.OnShoulderAim -= SetShoulderAimState;
            _controller.OnLand -= SetAnimLand;

            _attackController.OnIdle -= SetIdle;
            _attackController.OnAim -= SetAim;
            _attackController.OnReloadStart -= SetReload;
        }

        private void Update()
        {
            UpdateAim();
            UpdateShoulderAimLayer();
        }

        #endregion
        
        #region 成员方法
        
        private void SetAnimMoveInputTarget(Vector2 inputDir)
        {
            _targetMoveInput = inputDir;
        }

        private void SetAnimJump(int times)
        {
            _animator.SetTrigger(_jumpHash);
            _animator.CrossFade("Jump", 0.1f, 0, 0f);
        }

        private void SetAnimLand()
        {
            _animator.CrossFade("Locomotion", 0.1f);
        }

        private void SetReload()
        {
            if (_isReloading) return; 

            _isReloading = true;
            _animator.SetBool("IsReload", true);

        }
        private void SetIdle()
        {
            _animator.SetBool("IsAim", false);
        }

        private void SetAim()
        {
            _animator.SetBool("IsAim", true);
        }
       
        private void SetShoulderAimState(bool isAim) => _targetShoulderLayerWeight = isAim ? 1f : 0;

        /// <summary>
        /// 插值过渡动画状态机参数
        /// </summary>
        private void UpdateAim()
        {
            _currentMoveInput = Vector2.Lerp(
                _currentMoveInput,
                _targetMoveInput,
                moveAnimSmooth * Time.deltaTime
            );

            _animator.SetFloat(_moveZHash, _currentMoveInput.y);
            _animator.SetFloat(_moveXHash, _currentMoveInput.x);
        }
        
        /// <summary>
        /// 插值过渡层级状态
        /// </summary>
        private void UpdateShoulderAimLayer()
        {
            _currentShoulderLayerWeight = Mathf.Lerp(
                _currentShoulderLayerWeight,
                _targetShoulderLayerWeight,
                shoulderAimLayerSmooth * Time.deltaTime
            );

            _animator.SetLayerWeight(
                shoulderAimLayerIndex,
                _currentShoulderLayerWeight
            );
        }

        /// <summary>
        /// 简易实现换弹动画
        /// </summary>
        // 以下方法由动画事件调用
        private void SetReloadAnimationWeight()
        {
            if (!_isReloading) return;
        }

        private void SetReloadAnimationComplete()
        {
            if (!_isReloading) return; 

            _isReloading = false;
            _animator.SetBool("IsReload", false);
        }

        #endregion
    }
}
