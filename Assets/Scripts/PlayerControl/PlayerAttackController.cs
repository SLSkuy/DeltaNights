using System;
using InputProcess;
using SkillSystem;
using SkillSystem.Skill;
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

        // 子控制器
        private PlayerWeaponController _weaponController;
        private PlayerSkillController _skillController;

        // 输入缓存
        private float _lastAttackValue;
        private float _lastActiveSkillValue;
        private float _lastUltimateSkillValue;
        
        #region 事件

        public event Action<WeaponData> OnSwitchWeapon; // 切换武器    
        public event Action OnAttackPressed;    // 攻击按键按下
        public event Action OnAttackReleased;   // 攻击按键释放
        
        public event Action<SkillType> OnSkillPressed;   // 技能键按下
        public event Action<SkillType> OnSkillReleased;

        #endregion

        #region 周期函数

        private void Awake()
        {
            // 初始化
            _weaponController = new PlayerWeaponController(this);
            _weaponController.SwitchWeapon(mainWeapon);
            
            _skillController = new PlayerSkillController(this);
            
            // 示例：注册技能
            var dashSkill = new DashSkill();
            dashSkill.Init(gameObject);
            _skillController.RegisterSkill(SkillType.ActiveSkill, dashSkill);
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
            _skillController.Tick(Time.deltaTime);
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
                OnAttackPressed?.Invoke();
            }
            
            // 攻击松开
            if (_lastAttackValue > 0f && attack <= 0f)
            {
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
                OnSkillPressed?.Invoke(SkillType.ActiveSkill);
            }

            // 主动技能键松开
            if (_lastActiveSkillValue > 0f && activeSkill <= 0f)
            {
                OnSkillReleased?.Invoke(SkillType.ActiveSkill);
            }

            // ===== 终极技能 =====
            if (_lastUltimateSkillValue <= 0f && ultimateSkill > 0f)
            {
                OnSkillPressed?.Invoke(SkillType.UltimateSkill);
            }

            // 终极技能松开
            if (_lastUltimateSkillValue > 0f && ultimateSkill <= 0f)
            {
                OnSkillReleased?.Invoke(SkillType.UltimateSkill);
            }

            // 记录上一帧输入
            _lastActiveSkillValue = activeSkill;
            _lastUltimateSkillValue = ultimateSkill;
        }
        
        #endregion
    }
}
