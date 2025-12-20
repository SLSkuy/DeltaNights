using System.Collections.Generic;
using UnityEngine;
using SkillSystem;

namespace PlayerControl
{
    /// <summary>
    /// 玩家技能控制器
    /// </summary>
    public class PlayerSkillController : MonoBehaviour
    {
        [SerializeField] private List<MonoBehaviour> skillBehaviours;

        private readonly Dictionary<SkillType, ISkill> _skills = new();

        #region 生命周期

        private void Awake()
        {
            foreach (var mb in skillBehaviours)
            {
                if (mb is ISkill skill)
                {
                    skill.Init(gameObject);
                    _skills.Add(skill.Type, skill);
                }
            }
        }

        private void Update()
        {
            float dt = Time.deltaTime;
            foreach (var skill in _skills.Values)
            {
                skill.SkillUpdate(dt);
            }
        }

        #endregion

        #region 事件绑定

        public void Bind(PlayerAttackController attackController)
        {
            attackController.OnActiveSkillArmed += () => UseArmedSkill(SkillType.ActiveSkill);
            attackController.OnActiveSkillReleased += () => ReleaseSkill(SkillType.ActiveSkill);
            attackController.OnActiveSkillCanceled += () => CancelSkill(SkillType.ActiveSkill);

            attackController.OnUltimateSkillArmed += () => UseArmedSkill(SkillType.UltimateSkill);
            attackController.OnUltimateSkillReleased += () => ReleaseSkill(SkillType.UltimateSkill);
            attackController.OnUltimateSkillCanceled += () => CancelSkill(SkillType.UltimateSkill);
        }

        #endregion

        #region 技能行为

        private void UseArmedSkill(SkillType type)
        {
            if (_skills.TryGetValue(type, out var skill))
            {
                skill.SkillArmed();
            }
        }

        private void ReleaseSkill(SkillType type)
        {
            if (_skills.TryGetValue(type, out var skill))
            {
                skill.SkillUsed();
            }
        }

        private void CancelSkill(SkillType type)
        {
            if (_skills.TryGetValue(type, out var skill))
            {
                skill.SkillUnarmed();
            }
        }

        #endregion
    }
}
