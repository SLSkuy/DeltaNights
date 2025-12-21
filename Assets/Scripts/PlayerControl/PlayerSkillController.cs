using System;
using System.Collections.Generic;
using SkillSystem;
using UnityEngine;

namespace PlayerControl
{
    /// <summary>
    /// 单个技能实例
    /// </summary>
    public class SkillEntry
    {
        public ISkill Skill;
        public float SkillCountDown;
        public int CurrentCharges;
    }
    
    /// <summary>
    /// 玩家技能控制器
    /// </summary>
    public class PlayerSkillController
    {
        private readonly PlayerAttackController _attackController;
        
        private readonly Dictionary<SkillType, SkillEntry> _skillEntries = new();
        private ISkill _currentSkill;   // 当前选中技能
        
        private bool _isFinished = true;    // 当前技能是否已释放完毕

        private bool _isSkillArmed;
        private bool _instantSkillHolding;

        public event Action<bool> OnSkillArmed; // 是否处于准备释放技能状态
        
        public PlayerSkillController(PlayerAttackController attackController)
        {
            _attackController = attackController;

            _attackController.OnAttackPressed += TrySkillUsed;
            _attackController.OnSkillPressed += InstantSkillProcess;
            _attackController.OnSkillReleased += InstantSkillUsed;
        }

        ~PlayerSkillController()
        {
            _attackController.OnAttackPressed -= TrySkillUsed;
            _attackController.OnSkillPressed -= InstantSkillProcess;
            _attackController.OnSkillReleased -= InstantSkillUsed;

            // 取消事件订阅
            foreach (var skillEntry in _skillEntries.Values)
            {
                skillEntry.Skill.OnFinished -= SkillFinished;
            }
        }
        
        /// <summary>
        /// 逻辑更新
        /// </summary>
        public void Tick(float deltaTime)
        {
            UpdateCountDown(deltaTime);
            
            // 当前已选中技能，且技能并没有释放完成，且如果为瞬发技能并没有释放技能按键
            if (_currentSkill != null && !_isFinished && !_instantSkillHolding)
            {
                _currentSkill.SkillUpdate(deltaTime);
            }
        }
        
        /// <summary>
        /// 注册技能信息
        /// </summary>
        /// <param name="skillType">技能类型</param>
        /// <param name="skill">技能实例</param>
        public void RegisterSkill(SkillType skillType, ISkill skill)
        {
            SkillEntry entry = new SkillEntry()
            {
                Skill = skill,
                CurrentCharges = 1,
                SkillCountDown = skill.Cooldown
            };

            if (_skillEntries.TryAdd(skillType, entry))
            {
                entry.Skill.OnFinished += SkillFinished;
            }
        }

        /// <summary>
        /// 更新冷却时间
        /// </summary>
        private void UpdateCountDown(float deltaTime)
        {
            foreach (var entry in _skillEntries.Values)
            {
                // 无法恢复的技能，取消冷却逻辑计算
                if (entry.Skill.LimitedUseSkill) continue;
                
                // 技能存储已达上限，不进行冷却计算
                if (entry.CurrentCharges == entry.Skill.MaxCharges) return;
                
                if (entry.SkillCountDown > 0)
                {
                    entry.SkillCountDown -= deltaTime;
                }
                else
                {
                    // 恢复技能使用次数，重置冷却时间
                    entry.CurrentCharges++;
                    entry.SkillCountDown = entry.Skill.Cooldown;
                    Debug.Log("技能恢复");
                }
            }
        }

        /// <summary>
        /// 瞬发与非瞬发技能类型处理
        /// </summary>
        private void InstantSkillProcess(SkillType skillType)
        {
            // 技能次数不足
            if (_skillEntries[skillType].CurrentCharges <= 0) return;

            // 当前是非瞬发技能，且已进入准备状态，再次按下取消准备状态
            if (_isSkillArmed && _currentSkill != null &&
                !_currentSkill.IsInstant && _currentSkill.Type == skillType)
            {
                _isSkillArmed = false;
                SkillUnarmed(skillType);
                OnSkillArmed?.Invoke(false);
                return;
            }

            // 同时只有使用一个技能
            if (_currentSkill != null) return;
            
            _currentSkill = _skillEntries[skillType].Skill;

            if (_currentSkill.IsInstant)
            {
                _instantSkillHolding = true;
                SkillArmed(skillType);
            }
            else
            {
                _isSkillArmed = true;
                SkillArmed(skillType);
                OnSkillArmed?.Invoke(true);
            }
        }


        /// <summary>
        /// 攻击键按下时，判断是否已经处于准备释放技能状态，若是，则释放技能
        /// </summary>
        /// <param name="skillType"></param>
        private void TrySkillUsed()
        {
            if (_isSkillArmed && _currentSkill is { IsInstant: false })
            {
                var entry = _skillEntries[_currentSkill.Type];

                // 次数校验
                if (entry.CurrentCharges <= 0) return;
            
                entry.CurrentCharges--;
                _isSkillArmed = false;
            
                _currentSkill?.SkillUsed();
            }
        }

        /// <summary>
        /// 技能释放完毕回调函数
        /// </summary>
        private void SkillFinished()
        {
            if(!_currentSkill.IsInstant) OnSkillArmed?.Invoke(false);
            
            _isFinished = true;
            _currentSkill = null;
        }
        
        #region 技能逻辑

        private void SkillArmed(SkillType skillType)
        {
            if (!_isFinished) return;

            _isFinished = false;
            _currentSkill = _skillEntries[skillType].Skill;
            _currentSkill.SkillArmed();
        }

        private void SkillUnarmed(SkillType skillType)
        {
            _currentSkill.SkillUnarmed();
            _isFinished = true;
            
            _currentSkill = null;
        }

        private void InstantSkillUsed(SkillType skillType)
        {
            if (!_instantSkillHolding) return;

            var entry = _skillEntries[skillType];
            if (entry.CurrentCharges <= 0) return;

            entry.CurrentCharges--;
            _instantSkillHolding = false;

            _currentSkill?.SkillUsed();
        }

        #endregion
    }
}
