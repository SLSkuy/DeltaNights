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