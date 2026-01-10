/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.19
 *  LastUpdate:  2025.12.21
 * 
 *  功能简述：
 *  PlayerAttackController 负责将玩家输入抽象为攻击与技能事件，
 *  并协调武器系统与技能系统的执行逻辑，是战斗输入的核心调度模块。
 *
 *  主要功能：
 *  - 监听攻击 / 技能输入并识别“按下 / 松开”边沿事件
 *  - 将输入转换为标准化的攻击与技能事件对外分发
 *  - 管理武器控制器（PlayerWeaponController）的生命周期与更新
 *  - 管理技能控制器（PlayerSkillController）的初始化与冷却更新
 *  - 在技能蓄力或释放期间对武器攻击进行锁定控制
 *
 *  设计说明：
 *  - 输入来源通过 IAttackSkillInputSource 接口注入，支持本地 / AI / 网络复用
 *  - 攻击与技能采用“按键边沿触发”机制，避免帧级重复触发
 *  - 控制器本身不直接执行具体攻击或技能逻辑，仅负责事件调度
 *  - 武器系统与技能系统解耦，通过事件与接口进行协作
 *
 *  使用说明：
 *  - 需在 Inspector 中配置主武器与技能配置（SkillConfig）
 *  - 场景启动时会自动初始化武器与技能控制器
 *  - 外部系统（动画、音效、特效等）应通过事件监听响应攻击与技能行为
 *  - 不建议在本类中直接编写具体伤害或表现逻辑
 * ------------------------------------------------------------ */

using System;
using InputProcess;
using SkillSystem;
using Unity.Cinemachine;
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
        [SerializeField] private WeaponData secondaryWeapon;//副武器
        [SerializeField] private WeaponData meleeWeapon;//近战武器

        [Header("技能配置")]
        [SerializeField] private SkillConfig activeSkill;
        [SerializeField] private SkillConfig ultimateSkill;
        
        // 输入来源（可替换为AI / 网络）
        private IAttackSkillInputSource _attackInputSource;

        // 子控制器
        private PlayerWeaponController _weaponController;
        private PlayerSkillController _skillController;
        private PlayerController _playerController; 

        // 输入缓存
        private float _lastAttackValue;
        private float _lastActiveSkillValue;
        private float _lastUltimateSkillValue;
        private float _lastSwitch1Value;
        private float _lastSwitch2Value;
        private float _lastSwitch3Value;

        private float _lastReloadValue;
        private bool _isReloading = false;

        #region 事件

        public event Action<WeaponData, PlayerController> OnSwitchWeapon; // 切换武器    
        public event Action OnAttackPressed;    // 攻击按键按下
        public event Action OnAttackReleased;   // 攻击按键释放
        
        public event Action<SkillType> OnSkillPressed;   // 技能键按下
        public event Action<SkillType> OnSkillReleased;

        public event Action OnReloadStart;
        public event Action OnReloadComplete;
        public event Action OnAim;
        public event Action OnIdle;
        public event Action OnRifleSwitch;
        #endregion

        #region 周期函数

        private void Awake()
        {
            _playerController = GetComponent<PlayerController>();
            // 武器控制器初始化
            _weaponController = new PlayerWeaponController(this);
            _weaponController.SwitchWeapon(mainWeapon,_playerController);
            _weaponController.OnIdle += ChangeToIDle;
            _weaponController.OnReloadComplete += OnWeaponReloadComplete;

            // 技能控制器初始化
            _skillController = new PlayerSkillController(this);
            _skillController.OnSkillArmed += _weaponController.SetAttackLock;
            _skillController.InitSkills(gameObject, new []{activeSkill, ultimateSkill});
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
            HandleReloadInput();
            HandleSwitchInput();

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
                OnAim?.Invoke();
                OnAttackPressed?.Invoke();
            }
            
            // 攻击松开
            if (_lastAttackValue > 0f && attack <= 0f)
            {
                OnAttackReleased?.Invoke();
            }

            _lastAttackValue = attack;
        }

        private void ChangeToIDle()
        {
            OnIdle?.Invoke();
        }
        
        /// <summary>
        /// 处理技能输入
        /// </summary>
        private void HandleSkillInput()
        {
            float active = _attackInputSource.ActiveSkill;
            float ultimate = _attackInputSource.UltimateSkill;

            // ===== 主动技能 =====
            if (_lastActiveSkillValue <= 0f && active > 0f)
            {
                OnSkillPressed?.Invoke(SkillType.ActiveSkill);
            }

            // 主动技能键松开
            if (_lastActiveSkillValue > 0f && active <= 0f)
            {
                OnSkillReleased?.Invoke(SkillType.ActiveSkill);
            }

            // ===== 终极技能 =====
            if (_lastUltimateSkillValue <= 0f && ultimate > 0f)
            {
                OnSkillPressed?.Invoke(SkillType.UltimateSkill);
            }

            // 终极技能松开
            if (_lastUltimateSkillValue > 0f && ultimate <= 0f)
            {
                OnSkillReleased?.Invoke(SkillType.UltimateSkill);
            }

            // 记录上一帧输入
            _lastActiveSkillValue = active;
            _lastUltimateSkillValue = ultimate;
        }

        /// <summary>
        /// 处理换弹输入
        /// </summary>
        private void HandleReloadInput()
        {
            float reload = _attackInputSource.Reload;

            // 手动换弹
            if (_lastReloadValue <= 0f && reload > 0f)
            {
                if (!_isReloading &&
                    mainWeapon._currentBulletNum < mainWeapon._bulletCapacity &&
                    mainWeapon._bulletTotal > 0)
                {
                    OnReloadStart?.Invoke();
                    _isReloading = true;
                }
            }
            // 自动换弹
            else if (mainWeapon._currentBulletNum <= 0 &&
                     mainWeapon._bulletTotal > 0 &&
                     !_isReloading)
            {
                OnReloadStart?.Invoke();
                _isReloading = true;
            }

            _lastReloadValue = reload;
        }

        private void OnWeaponReloadComplete()
        {
            //供外部订阅
            OnReloadComplete?.Invoke();
            _isReloading = false;
        }

        /// <summary>
        /// 处理切枪输入
        /// </summary>
        private void HandleSwitchInput()
        {
            float switch1 = _attackInputSource.Switch1;
            float switch2 = _attackInputSource.Switch2;
            float switch3 = _attackInputSource.Switch3;

            if (_lastSwitch1Value <= 0f && switch1 > 0f)  
            {
                if (mainWeapon != null)
                {
                    _weaponController.SwitchWeapon(mainWeapon, _playerController);
                    OnRifleSwitch?.Invoke();
                }
            }

            if (_lastSwitch2Value <= 0f && switch2 > 0f)
            {
                if (secondaryWeapon != null)
                {
                    _weaponController.SwitchWeapon(secondaryWeapon, _playerController);
                    OnRifleSwitch?.Invoke();
                }
            }
            if (_lastSwitch3Value <= 0f && switch3 > 0f)
            {
                if (meleeWeapon != null)
                {
                    _weaponController.SwitchWeapon(meleeWeapon, _playerController);
                    OnRifleSwitch?.Invoke();
                }
            }

            _lastSwitch1Value = switch1;
            _lastSwitch2Value = switch2;
            _lastSwitch3Value = switch3;
        }

        #endregion
    }
}