using System;
using InputProcess;
using UnityEngine;
using WeaponSystem;
using WeaponSystem.Weapon;

namespace PlayerControl
{
    /// <summary>
    /// 测试用类，显示当前使用武器属性
    /// </summary>
    [Serializable]
    public class WeaponProperties
    {
        [Header("武器属性")] 
        public WeaponType weaponType;
        [Tooltip("主武器攻击速度")]
        public float attackSpeed;
        [Tooltip("是否为全自动武器")]
        public bool fullyAutomaticWeapon;

        [Header("伤害属性")] 
        public float attenuationFactor;
        public float headDamage;
        public float bodyDamage;
        public float legDamage;
    }

    /// <summary>
    /// 玩家攻击控制器
    /// 负责将输入转换为攻击/技能事件
    /// </summary>
    public class PlayerAttackController : MonoBehaviour
    {
        [Header("当前武器属性")] 
        public WeaponProperties currentWeapon;
        private IWeapon _currentWeapon;

        [Header("技能属性")] 
        [Tooltip("主动技能是否为瞬发性技能")]
        public bool instantActiveSkill;
        [Tooltip("终极技能是否为瞬发性技能")]
        public bool instantUltimateSkill;
        
        // 输入来源（可替换为AI / 网络）
        private IAttackSkillInputSource _attackInputSource;

        // 输入缓存
        private float _lastAttackValue;
        private float _lastActiveSkillValue;
        private float _lastUltimateSkillValue;
        private bool _activeSkillActivated;
        private bool _ultimateSkillActivated;
        
        // 是否进入技能准备状态（锁定输入）
        private bool _activeSkillArmed;
        private bool _ultimateSkillArmed;
        
        // 攻击事件
        private float _attackCooldown; // 攻击冷却计时器
        private bool _attackLockedBySkill;  // 技能释放后必须松开按键才能重新开始普通攻击
        
        #region 事件

        public event Action OnAttackPressed;    // 攻击按键按下
        public event Action OnAttackReleased;   // 攻击按键释放
        public event Action OnActiveSkillTrigger;   // 主动技能触发
        public event Action OnActiveSkillCanceled;  // 取消主动技能
        public event Action OnUltimateSkillTrigger; // 主动技能触发
        public event Action OnUltimateSkillCanceled;    // 取消主动技能
        public event Action<WeaponType> OnSwitchWeapon; // 切换武器    

        #endregion

        #region 周期函数

        private void Awake()
        {
            // 测试使用武器
            SwitchWeapon(new Rifle());
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
        }

        #endregion

        #region 成员方法

        /// <summary>
        /// 切换当前使用武器
        /// </summary>
        public void SwitchWeapon(IWeapon newWeapon)
        {
            if(newWeapon == null) return;
            
            _currentWeapon = newWeapon;
            
            currentWeapon.attackSpeed = _currentWeapon.AttackSpeed;
            currentWeapon.fullyAutomaticWeapon = _currentWeapon.FullyAutomatic;

            currentWeapon.attenuationFactor = _currentWeapon.AttenuationFactor;
            currentWeapon.headDamage = _currentWeapon.HeadDamage;
            currentWeapon.bodyDamage = _currentWeapon.BodyDamage;
            currentWeapon.legDamage = _currentWeapon.LegDamage;
            
            // 触发切换武器事件
            OnSwitchWeapon?.Invoke(_currentWeapon.WeaponType);
        }

        /// <summary>
        /// 尝试触发武器攻击
        /// </summary>
        private void TryWeaponAttack()
        {
            if (_currentWeapon == null) return;

            // 如果冷却未到，跳过
            if (_attackCooldown > 0f) return;

            // 调用武器攻击
            _currentWeapon.Attack();

            // 重置冷却时间（attackSpeed 表示每10秒攻击次数）
            if (_currentWeapon.AttackSpeed > 0f)
            {
                _attackCooldown = 10f / _currentWeapon.AttackSpeed;
            }
        }

        /// <summary>
        /// 强制打断所有技能状态，回到正常状态
        /// 用于处理击飞，死亡等事件
        /// </summary>
        public void ForceCancelSkills()
        {
            if (_activeSkillArmed)
            {
                _activeSkillArmed = false;
                OnActiveSkillCanceled?.Invoke();
            }

            if (_ultimateSkillArmed)
            {
                _ultimateSkillArmed = false;
                OnUltimateSkillCanceled?.Invoke();
            }
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
                    OnActiveSkillTrigger?.Invoke();
                    Debug.Log("主动技能释放");
                    return;
                }

                // 非瞬发终极技能：攻击键触发技能
                if (_ultimateSkillArmed && !instantUltimateSkill && !_activeSkillArmed)
                {
                    // 同时只能触发一个技能
                    _ultimateSkillArmed = false;

                    _attackLockedBySkill = true;
                    OnUltimateSkillTrigger?.Invoke();
                    Debug.Log("终极技能释放");
                    return;
                }
                
                OnAttackPressed?.Invoke();
            }
            
            // 持续按下（全自动武器）
            if (attack > 0f && _currentWeapon is { FullyAutomatic: true } && !_attackLockedBySkill)
            {
                TryWeaponAttack();
            }
            
            // 单击（半自动武器）
            if (attack > 0f && _currentWeapon is { FullyAutomatic : false } && !_attackLockedBySkill)
            {
                // 释放鼠标按键后再次按下才能进行设计
                if (_lastAttackValue <= 0f)
                {
                    TryWeaponAttack();
                }
            }

            // 攻击松开
            if (_lastAttackValue > 0f && attack <= 0f)
            {
                _attackLockedBySkill = false;   // 释放技能后，松开按键，解锁开火限制
                OnAttackReleased?.Invoke();
            }

            _lastAttackValue = attack;

            // 更新冷却计时器
            if (_attackCooldown > 0f)
            {
                _attackCooldown -= Time.deltaTime;
            }
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
                    OnActiveSkillTrigger?.Invoke();
                }
            }

            // ===== 终极技能 =====
            if (_lastUltimateSkillValue <= 0f && ultimateSkill > 0f)
            {
                if (!_ultimateSkillArmed)
                {
                    // 进入技能准备状态
                    _ultimateSkillArmed = true;
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
                    OnUltimateSkillTrigger?.Invoke();
                }
            }

            // 记录上一帧输入
            _lastActiveSkillValue = activeSkill;
            _lastUltimateSkillValue = ultimateSkill;
        }
        
        #endregion
    }
}
