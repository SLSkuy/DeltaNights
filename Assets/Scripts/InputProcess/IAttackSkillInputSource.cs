/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.19
 *  LastUpdate: 2025.12.19
 * 
 *  功能简述：
 *  IAttackSkillInputSource 定义玩家攻击与技能输入的统一访问接口。
 *
 *  使用说明：
 *  - 用于向攻击系统与技能系统提供输入数据
 *  - 输入值仅用于状态判断，不包含具体触发逻辑
 *  - 实现类需保证输入数据的实时性与一致性
 * ------------------------------------------------------------ */

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
        float Reload { get; }
        float Switch1 { get; }
        float Switch2 { get; }
        float Switch3 { get; }

    }
}