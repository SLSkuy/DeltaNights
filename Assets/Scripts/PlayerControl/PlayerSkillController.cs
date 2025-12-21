using System.Collections.Generic;
using SkillSystem;

namespace PlayerControl
{
    /// <summary>
    /// 玩家技能控制器
    /// </summary>
    public class PlayerSkillController
    {
        private readonly PlayerAttackController _attackController;
        
        private readonly Dictionary<SkillType, ISkill> _skills = new Dictionary<SkillType, ISkill>();
        private ISkill _currentSkill;   // 当前选中技能
        
        private bool _isFinished = true;    // 当前技能是否已释放完毕

        private bool _isSkillArmed;
        private bool _instantSkillHolding;  
        
        public PlayerSkillController(PlayerAttackController attackController)
        {
            _attackController = attackController;

            _attackController.OnSkillPressed += InstantSkillProcess;
            _attackController.OnSkillReleased += SkillUsed;
        }

        ~PlayerSkillController()
        {
            _attackController.OnSkillPressed -= InstantSkillProcess;
            _attackController.OnSkillReleased -= SkillUsed;

            // 取消事件订阅
            foreach (var skills in _skills.Values)
            {
                skills.OnFinished -= SkillFinished;
            }
        }

        /// <summary>
        /// 注册技能信息
        /// </summary>
        /// <param name="skillType">技能类型</param>
        /// <param name="skill">技能实例</param>
        public void RegisterSkill(SkillType skillType, ISkill skill)
        {
            if (_skills.TryAdd(skillType, skill))
            {
                skill.OnFinished += SkillFinished;
            }
        }

        /// <summary>
        /// 逻辑更新
        /// </summary>
        public void Tick(float deltaTime)
        {
            // 当前已选中技能，且技能并没有释放完成，且如果为瞬发技能并没有释放技能按键
            if (_currentSkill != null && !_isFinished && !_instantSkillHolding)
            {
                _currentSkill.SkillUpdate(deltaTime);
            }
        }

        /// <summary>
        /// 瞬发与非瞬发技能类型处理
        /// </summary>
        private void InstantSkillProcess(SkillType skillType)
        {
            _currentSkill = _skills[skillType];

            if (_currentSkill.IsInstant)
            {
                _isFinished = true;
                _instantSkillHolding = true;
                SkillArmed(skillType);
            }
            else
            {
                // 非顺发性技能，进入准备状态
                if (_isSkillArmed)
                {
                    _isSkillArmed = false;
                    SkillUnarmed(skillType);
                }
                else
                {
                    _isSkillArmed = true;
                    SkillArmed(skillType);
                }
            }
        }

        /// <summary>
        /// 技能释放完毕回调函数
        /// </summary>
        private void SkillFinished()
        {
            _isFinished = true;
            _currentSkill = null;
        }
        
        #region 技能逻辑

        private void SkillArmed(SkillType skillType)
        {
            _isFinished = false;
            _currentSkill.SkillArmed();
        }

        private void SkillUnarmed(SkillType skillType)
        {
            _currentSkill.SkillUnarmed();
            _isFinished = true;
            
            _currentSkill = null;
        }

        private void SkillUsed(SkillType skillType)
        {
            if (_instantSkillHolding)
            {
                _instantSkillHolding = false;
                _currentSkill.SkillUsed();
            }
        }
        
        #endregion
    }
}
