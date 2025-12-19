using System;
using InputProcess;
using UnityEngine;

namespace PlayerControl
{
    /// <summary>
    /// 玩家攻击控制器
    /// 负责将输入转换为攻击/技能事件
    /// </summary>
    public class PlayerAttackController : MonoBehaviour
    {
        [Header("攻击属性")] 
        [Tooltip("主武器攻击速度")]
        public float attackSpeed;
        [Tooltip("是否为全自动武器")]
        public bool fullyAutomaticWeapon;

        [Header("技能属性")] 
        [Tooltip("主动技能是否为瞬发性技能")]
        public bool instantActiveSkill;
        [Tooltip("终极技能是否为瞬发性技能")]
        public bool instantUltimateSkill;
        
        // 输入来源（可替换为AI / 网络）
        private IAttackSkillSource _attackInputSource;

        // 输入缓存
        private float _lastAttackValue;
        private float _lastActiveSkillValue;
        private float _lastUltimateSkillValue;
        private bool _activeSkillActivated;
        private bool _ultimateSkillActivated;
        
        // 是否进入技能准备状态（锁定输入）
        private bool _activeSkillArmed;
        private bool _ultimateSkillArmed;
        
        #region 事件

        public event Action OnAttackPressed;    // 攻击按键按下
        public event Action OnAttackReleased;   // 攻击按键释放
        public event Action OnActiveSkillTrigger;   // 主动技能触发
        public event Action OnActiveSkillCanceled;  // 取消主动技能
        public event Action OnUltimateSkillTrigger; // 主动技能触发
        public event Action OnUltimateSkillCanceled;    // 取消主动技能

        #endregion

        #region 周期函数

        private void Awake()
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
        /// 处理攻击输入
        /// </summary>
        private void HandleAttackInput()
        {
            float attack = _attackInputSource.Attack;

            // 攻击按下
            if (_lastAttackValue <= 0f && attack > 0f)
            {
                // 非瞬发主动技能：攻击键触发并返回
                if (_activeSkillArmed && !instantActiveSkill)
                {
                    _activeSkillArmed = false;
                    OnActiveSkillTrigger?.Invoke();
                    return;
                }

                // 非瞬发终极技能：攻击键触发并返回
                if (_ultimateSkillArmed && !instantUltimateSkill)
                {
                    _ultimateSkillArmed = false;
                    OnUltimateSkillTrigger?.Invoke();
                    return;
                }

                // 普通攻击
                OnAttackPressed?.Invoke();
            }

            // 攻击松开
            if (_lastAttackValue > 0f && attack <= 0f)
            {
                OnAttackReleased?.Invoke();
            }

            // 记录上一帧输入
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
                }
                else
                {
                    // 取消技能释放（仅非瞬发技能）
                    _activeSkillArmed = false;
                    OnActiveSkillCanceled?.Invoke();
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
                }
                else
                {
                    // 取消技能释放（仅非瞬发技能）
                    _ultimateSkillArmed = false;
                    OnUltimateSkillCanceled?.Invoke();
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
    }
}
