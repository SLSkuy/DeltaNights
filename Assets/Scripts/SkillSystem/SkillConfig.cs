/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.21
 *  LastUpdate: 2025.12.21
 * 
 *  功能简述：
 *  SkillConfig 为技能系统的抽象配置类，
 *  用于以数据驱动方式定义技能的通用参数。
 *
 *  主要功能：
 *  - 通过 ScriptableObject 存储技能基础配置
 *  - 描述技能类型、冷却、使用限制及存储数量
 *  - 提供运行时技能实例的创建接口
 *
 *  使用说明：
 *  - 不直接参与技能逻辑执行
 *  - 需由具体技能配置类继承并实现 CreateSkill
 *  - 技能系统通过该配置生成对应的运行时技能对象
 * ------------------------------------------------------------ */

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