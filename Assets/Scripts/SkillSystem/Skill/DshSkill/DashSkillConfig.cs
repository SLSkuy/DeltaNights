/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.21
 *  LastUpdate:  2025.12.21
 * 
 *  功能简述：
 *  DashSkillConfig 为冲刺（Dash）技能的配置类，
 *  通过 ScriptableObject 定义技能参数，并负责生成具体技能实例。
 *
 *  主要功能：
 *  - 配置冲刺持续时间（dashDuration）
 *  - 配置冲刺施加的力或速度（dashForce）
 *  - 作为技能数据载体，与运行时逻辑解耦
 *
 *  设计说明：
 *  - 继承自 SkillConfig，用于瞬发型技能（Instant Skill）
 *  - 本类不包含具体执行逻辑，仅负责参数配置
 *  - 运行时逻辑由 DashSkill 类实现
 *  - 可通过 CreateAssetMenu 直接在编辑器中创建技能配置
 *
 *  使用说明：
 *  - 在 Unity 编辑器中创建 DashSkillConfig 资源
 *  - 配置冲刺时间与力度参数
 *  - 由技能系统在运行时调用 CreateSkill 生成 DashSkill 实例
 * ------------------------------------------------------------ */

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