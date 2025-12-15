using UnityEngine;

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
        
        private PlayerController _controller;
        private Animator _animator;

        private float _currentShoulderLayerWeight;
        private float _targetShoulderLayerWeight;
        
        private Vector2 _currentMoveInput;
        private Vector2 _targetMoveInput;
        private int _moveZHash;
        private int _moveXHash;

        #region 周期函数

        private void Awake()
        {
            _controller = GetComponent<PlayerController>();
            _animator = GetComponent<Animator>();
            
            _moveZHash = Animator.StringToHash("MoveZ");
            _moveXHash = Animator.StringToHash("MoveX");
        }

        private void Start()
        {
            _controller.OnMove += SetAnimMoveInputTarget;
            _controller.OnJump += SetAnimJump;
            _controller.OnShoulderAim += SetShoulderAimState;
        }

        private void OnDestroy()
        {
            _controller.OnMove -= SetAnimMoveInputTarget;
            _controller.OnJump -= SetAnimJump;
            _controller.OnShoulderAim -= SetShoulderAimState;
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
            _animator.SetTrigger("Jump");
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
        
        #endregion
    }
}
