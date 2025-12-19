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