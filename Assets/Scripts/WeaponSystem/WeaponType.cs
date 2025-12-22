/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.19
 *  LastUpdate: 2025.12.19
 * 
 *  功能简述：
 *  WeaponType 用于标识武器在武器系统中的分类类型。
 *
 *  使用说明：
 *  - 用于区分主武器、副武器与近战武器
 *  - 作为武器配置、切换与逻辑判断的基础标识
 * ------------------------------------------------------------ */

namespace WeaponSystem
{
    public enum WeaponType
    {
        MainWeapon,
        SecondaryWeapon,
        MeleeWeapon
    }
}