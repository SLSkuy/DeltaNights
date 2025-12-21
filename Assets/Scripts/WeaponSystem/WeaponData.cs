using UnityEngine;

namespace WeaponSystem
{
    public abstract class WeaponData : ScriptableObject, IWeapon
    {
        [Header("武器类型")]
        [SerializeField] protected WeaponType weaponType;
        [SerializeField] protected string weaponName;

        [Header("武器性能")]
        [SerializeField] protected float attackSpeed;
        [SerializeField] protected bool fullyAutomatic;
        [SerializeField] protected float attenuationFactor;

        [Header("武器伤害")]
        [SerializeField] protected float headDamage;
        [SerializeField] protected float bodyDamage;
        [SerializeField] protected float legDamage;

        public WeaponType WeaponType => weaponType;
        public string WeaponName => weaponName;
        public float AttackSpeed => attackSpeed;
        public bool FullyAutomatic => fullyAutomatic;
        public float AttenuationFactor => attenuationFactor;
        public float HeadDamage => headDamage;
        public float BodyDamage => bodyDamage;
        public float LegDamage => legDamage;

        /// <summary>
        /// 具体攻击逻辑实现，根据不同武器添加额外属性，如实现霰弹枪、步枪等不同类型枪械
        /// </summary>
        public abstract void Attack();
    }
}