using System;
using InputProcess;
using UnityEngine;
using WeaponSystem;

namespace PlayerControl
{
    /// <summary>
    /// 玩家攻击控制器
    /// 负责将输入转换为攻击/技能事件
    /// </summary>
    public class PlayerAttackController : MonoBehaviour
    {
        [Header("武器配置")] 
        [SerializeField] private WeaponData mainWeapon;
        
        [Header("技能属性")] 
        [Tooltip("主动技能是否为瞬发性技能")]
        public bool instantActiveSkill;
        [Tooltip("终极技能是否为瞬发性技能")]
        public bool instantUltimateSkill;
        
        // 输入来源（可替换为AI / 网络）
        private IAttackSkillInputSource _attackInputSource;

        private PlayerWeaponController _weaponController;

        // 输入缓存
        private float _lastAttackValue;
        private float _lastActiveSkillValue;
        private float _lastUltimateSkillValue;
        
        // 是否进入技能准备状态（锁定输入）
        private bool _activeSkillArmed;
        private bool _ultimateSkillArmed;
        
        // 攻击事件
        private bool _attackLockedBySkill;  // 技能释放后必须松开按键才能重新开始普通攻击
        
        #region 事件

        public event Action OnAttackPressed;    // 攻击按键按下
        public event Action OnAttackReleased;   // 攻击按键释放
        public event Action OnActiveSkillArmed;   // 主动技能触发
        public event Action OnActiveSkillReleased;  // 主动技能释放
        public event Action OnActiveSkillCanceled;  // 取消主动技能
        public event Action OnUltimateSkillArmed; // 终极技能触发
        public event Action OnUltimateSkillReleased;    // 终极技能释放
        public event Action OnUltimateSkillCanceled;    // 取消主动技能
        public event Action<WeaponData> OnSwitchWeapon; // 切换武器    

        #endregion

        #region 周期函数

        private void Awake()
        {
            // 初始化
            _weaponController = new PlayerWeaponController(this);
            _weaponController.SwitchWeapon(mainWeapon);
        }

        private void Start()
        {
            // 输入注入
            _attackInputSource = GameInput.Instance;
        }

        private void Update()
        {
            if (_attackInputSource == null)
                return;
            
            HandleSkillInput();
            HandleAttackInput();
            
            _weaponController.Tick(Time.deltaTime);
        }

        #endregion
        
        #region 输入处理
        
        /// <summary>
        /// 处理攻击输入
        /// </summary>
        private void HandleAttackInput()
        {
            float attack = _attackInputSource.Attack;

            // 攻击按下
            if (_lastAttackValue <= 0f && attack > 0f)
            {
                // 非瞬发主动技能：攻击键触发技能
                if (_activeSkillArmed && !instantActiveSkill && !_ultimateSkillArmed)
                {
                    // 同时只能触发一个技能
                    _activeSkillArmed = false;
                    
                    _attackLockedBySkill = true;
                    OnActiveSkillReleased?.Invoke();
                    Debug.Log("主动技能释放");
                    return;
                }

                // 非瞬发终极技能：攻击键触发技能
                if (_ultimateSkillArmed && !instantUltimateSkill && !_activeSkillArmed)
                {
                    // 同时只能触发一个技能
                    _ultimateSkillArmed = false;
                    
                    _attackLockedBySkill = true;
                    OnUltimateSkillReleased?.Invoke();
                    Debug.Log("终极技能释放");
                    return;
                }
                
                if(!_attackLockedBySkill)
                    OnAttackPressed?.Invoke();
            }
            
            // 攻击松开
            if (_lastAttackValue > 0f && attack <= 0f)
            {
                _attackLockedBySkill = false;   // 释放技能后，松开按键，解锁开火限制
                OnAttackReleased?.Invoke();
            }

            _lastAttackValue = attack;
        }
        
        /// <summary>
        /// 处理技能输入
        /// </summary>
        private void HandleSkillInput()
        {
            float activeSkill = _attackInputSource.ActiveSkill;
            float ultimateSkill = _attackInputSource.UltimateSkill;

            // ===== 主动技能 =====
            if (_lastActiveSkillValue <= 0f && activeSkill > 0f)
            {
                // 技能键按下
                if (!_activeSkillArmed)
                {
                    // 进入技能准备状态
                    _activeSkillArmed = true;
                    OnActiveSkillArmed?.Invoke();
                    Debug.Log("主动技能触发");
                }
                else
                {
                    // 取消技能释放（仅非瞬发技能）
                    _activeSkillArmed = false;
                    OnActiveSkillCanceled?.Invoke();
                    Debug.Log("主动技能取消");
                }
            }

            // 主动技能键松开
            if (_lastActiveSkillValue > 0f && activeSkill <= 0f)
            {
                // 主动技能触发（仅瞬发技能）
                if (_activeSkillArmed && instantActiveSkill)
                {
                    _activeSkillArmed = false;
                    OnActiveSkillReleased?.Invoke();
                    Debug.Log("瞬发性主动技能释放");
                }
            }

            // ===== 终极技能 =====
            if (_lastUltimateSkillValue <= 0f && ultimateSkill > 0f)
            {
                if (!_ultimateSkillArmed)
                {
                    // 进入技能准备状态
                    _ultimateSkillArmed = true;
                    OnUltimateSkillArmed?.Invoke();
                    Debug.Log("终极技能触发");
                }
                else
                {
                    // 取消技能释放（仅非瞬发技能）
                    _ultimateSkillArmed = false;
                    OnUltimateSkillCanceled?.Invoke();
                    Debug.Log("终极技能取消");
                }
            }

            // 终极技能松开
            if (_lastUltimateSkillValue > 0f && ultimateSkill <= 0f)
            {
                // 终极技能触发（仅瞬发技能）
                if (_ultimateSkillArmed && instantUltimateSkill)
                {
                    _ultimateSkillArmed = false;
                    OnUltimateSkillReleased?.Invoke();
                    Debug.Log("瞬发性终极技能释放");
                }
            }

            // 记录上一帧输入
            _lastActiveSkillValue = activeSkill;
            _lastUltimateSkillValue = ultimateSkill;
        }
        
        #endregion
    }
}
