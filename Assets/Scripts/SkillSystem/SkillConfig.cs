using UnityEngine;

namespace SkillSystem
{
    /// <summary>
    /// 技能抽象类，定义技能通用属性，继承ScriptableObject便于数据管理
    /// </summary>
    public abstract class SkillConfig : ScriptableObject
    {
        [Header("通用技能配置")]
        public SkillType skillType;
        public bool isInstant;
        public float cooldown;
        public bool limitedUseSkill;
        public int maxCharges;

        // 创建运行时技能
        public abstract ISkill CreateSkill();
    }
}