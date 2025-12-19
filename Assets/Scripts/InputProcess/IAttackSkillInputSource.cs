namespace InputProcess
{
    /// <summary>
    /// 玩家攻击及技能输入接口
    /// </summary>
    public interface IAttackSkillInputSource
    {
        float Attack { get; }
        float ActiveSkill { get; }
        float UltimateSkill { get; }
    }
}