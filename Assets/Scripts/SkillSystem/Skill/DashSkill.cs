using UnityEngine;

namespace SkillSystem.Skill
{
    public class DashSkill : ISkill
    {
        public SkillType Type => SkillType.ActiveSkill;
        public bool IsInstant => true;
        public float Cooldown => 3f;
        public bool LimitedUseSkill => false;
        public int MaxCharges => 1;
        
        private int _currentCharges;
        private float _cooldownTimer;

        public void Init(GameObject player)
        {
            _currentCharges = MaxCharges;
        }
        
        public void SkillUpdate(float deltaTime)
        {
            _cooldownTimer -= deltaTime;
        }
        
        /// <summary>
        /// 进入技能准备状态
        /// </summary>
        public void SkillArmed()
        {
            if (_currentCharges <= 0)
                return;
            
            Debug.Log("Dash Skill Armed");
        }

        /// <summary>
        /// 取消技能
        /// </summary>
        public void SkillUnarmed()
        {
            // 瞬发性技能，无取消技能逻辑
        }

        /// <summary>
        /// 技能释放
        /// </summary>
        public void SkillUsed()
        {
            if (_currentCharges <= 0)
                return;
            
            _currentCharges--;

            // 启动冷却
            _cooldownTimer = Cooldown;

            Debug.Log($"Dash Used, Remaining Charges: {_currentCharges}");
        }
    }
}
