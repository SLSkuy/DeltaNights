/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.20
 *  LastUpdate: 2025.12.20
 * 
 *  功能简述：
 *  WeaponData 为武器系统的抽象数据基类，
 *  负责统一定义武器的基础配置与对外接口。
 *
 *  主要职责：
 *  - 以 ScriptableObject 形式保存武器配置数据
 *  - 提供武器类型、名称、射速、自动模式等基础属性
 *  - 统一管理不同命中部位的伤害数值
 *  - 约定 Attack 接口，由具体武器实现实际攻击逻辑
 *
 *  使用说明：
 *  - 本类仅负责“数据与接口定义”，不处理具体表现与逻辑
 *  - 新武器需继承 WeaponData 并实现 Attack 方法
 * ------------------------------------------------------------ */

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