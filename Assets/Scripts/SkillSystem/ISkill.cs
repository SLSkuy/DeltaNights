/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.20
 *  LastUpdate: 2025.12.21
 * 
 *  功能简述：
 *  ISkill 定义技能系统的统一接口规范，
 *  约束所有技能必须具备的基础属性与生命周期行为。
 *
 *  主要功能：
 *  - 描述技能类型、冷却、使用限制等核心属性
 *  - 统一技能的初始化、更新与释放流程
 *  - 提供技能释放完成事件，用于与外部系统解耦交互
 *
 *  使用说明：
 *  - 接口仅声明能力，不包含具体实现
 *  - 所有技能需实现完整的技能生命周期方法
 *  - 技能控制器通过该接口驱动技能逻辑与状态切换
 * ------------------------------------------------------------ */

using System;
using UnityEngine;

namespace SkillSystem
{
    /// <summary>
    /// 技能接口，定义技能的各种属性
    /// </summary>
    public interface ISkill
    
    {
        public SkillType Type { get; }
        
        /// <summary>
        /// 是否为瞬发性技能
        /// </summary>
        public bool IsInstant { get; }
        
        /// <summary>
        /// 技能冷却时间
        /// </summary>
        public float Cooldown { get; }
        
        /// <summary>
        /// 是否为限制使用技能（技能无法通过冷却恢复）
        /// </summary>
        public bool LimitedUseSkill { get; }
        
        /// <summary>
        /// 技能最大存储数量
        /// </summary>
        public int MaxCharges { get; }

        /// <summary>
        /// 技能释放完毕回调事件
        /// </summary>
        public event Action OnFinished;
        
        /// <summary>
        /// 技能逻辑更新接口
        /// </summary>
        public void SkillUpdate(float deltaTime);
        
        /// <summary>
        /// 初始化技能，以获取各种相应的组件
        /// </summary>
        /// <param name="player"></param>
        public void Init(GameObject player);
        
        public void SkillArmed();
        public void SkillUnarmed();
        public void SkillUsed();
    }
}