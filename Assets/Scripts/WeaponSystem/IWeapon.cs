/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.19
 *  LastUpdate: 2025.12.19
 * 
 *  功能简述：
 *  IWeapon 定义武器系统的统一接口规范，
 *  约束所有武器必须具备的基础属性与攻击行为。
 *
 *  主要作用：
 *  - 为武器逻辑、输入系统、战斗系统提供统一访问方式
 *  - 支持不同武器实现的多态替换，降低系统耦合
 *
 *  使用说明：
 *  - 接口仅声明能力，不包含具体实现
 *  - 所有武器实现（数据类或逻辑类）必须遵循该接口
 * ------------------------------------------------------------ */

namespace WeaponSystem
{
    /// <summary>
    /// 武器接口，用于定义武器的各种属性
    /// </summary>
    public interface IWeapon
    {
        public WeaponType WeaponType { get; }

        /// <summary>
        /// 武器攻击速度（每10秒攻击次数）
        /// </summary>
        public float AttackSpeed { get; }
        
        /// <summary>
        /// 武器是否为全自动武器（是否能够连续攻击）
        /// </summary>
        public bool FullyAutomatic { get; }

        /// <summary>
        /// 武器伤害随距离衰减因素
        /// </summary>
        public float AttenuationFactor { get; }

        public float HeadDamage { get; }
        public float BodyDamage { get; }
        public float LegDamage { get; }
        
        /// <summary>
        /// 武器攻击实现，处理具体的攻击逻辑
        /// </summary>
        public void Attack();
    }
}