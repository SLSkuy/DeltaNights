using UnityEngine;

namespace WeaponSystem.Weapon
{
    public class Rifle : IWeapon
    {
        public WeaponType WeaponType => WeaponType.MainWeapon;
        public float AttackSpeed => 120f;
        public bool FullyAutomatic => false;
        public float AttenuationFactor => 0.9f;
        public float HeadDamage => 30f;
        public float BodyDamage => 20f;
        public float LegDamage => 10f;

        public void Attack()
        {
            Debug.Log("Rifle attack fired!");
            // 这里可以实现子弹生成、射线检测、伤害计算等逻辑
        }
    }
}