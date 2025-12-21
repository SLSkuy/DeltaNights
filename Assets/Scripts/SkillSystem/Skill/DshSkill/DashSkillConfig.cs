using UnityEngine;

namespace SkillSystem.Skill.DshSkill
{
    /// <summary>
    /// 冲刺示例技能，继承SkillConfig实现额外逻辑
    /// </summary>
    [CreateAssetMenu(menuName = "Skill/InstantSkill/Dash Skill")]
    public class DashSkillConfig : SkillConfig
    {
        [Header("冲刺技能配置")]
        public float dashDuration = 0.2f;
        public float dashForce = 50f;

        public override ISkill CreateSkill()
        {
            return new DashSkill(this);
        }
    }
}