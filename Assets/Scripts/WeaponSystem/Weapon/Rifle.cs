/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.19
 *  LastUpdate: 2025.12.20
 * 
 *  功能简述：
 *  Rifle 为步枪类型的武器示例实现，
 *  继承 WeaponData 并提供具体的攻击行为。
 *
 *  主要功能：
 *  - 使用 WeaponData 中配置的基础武器属性
 *  - 实现步枪的开火行为（Attack）
 *
 *  使用说明：
 *  - 通过 CreateAssetMenu 在编辑器中创建 Rifle 资源
 *  - 在 Inspector 中配置射速、伤害等参数
 *  - 由武器控制或攻击系统调用 Attack 方法触发攻击
 * ------------------------------------------------------------ */

using UnityEngine;

namespace WeaponSystem.Weapon
{
    /// <summary>
    /// 步枪示例
    /// </summary>
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