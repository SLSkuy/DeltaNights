using UnityEngine;

namespace WeaponSystem.Weapon
{
    [CreateAssetMenu(fileName = "Rifle", menuName = "Weapon/Rifle")]
    public class Rifle : WeaponData
    {
        public override void Attack()
        {
            Debug.Log("Rifle Fired!");
            // 具体武器逻辑
        }
    }
}