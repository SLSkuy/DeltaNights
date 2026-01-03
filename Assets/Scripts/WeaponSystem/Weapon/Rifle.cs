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

using PlayerControl;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
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
            Debug.Log("playerController：" + _playerController);
            Debug.Log("isAim：" + _isAim);
            Debug.Log("isShoulderAim：" + _isShoulderAim);
            Debug.Log("rifle:" + _rifle);
            Debug.Log("Muzzle:" + _muzzle);
            //// 具体武器逻辑
            ////射线射向屏幕中心
            Ray centerRay = Camera.main.ViewportPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));
            Ray shoulderRay = new Ray(_muzzle.transform.position, centerRay.direction);//肩射射线
            Ray aimRay = new Ray(Camera.main.transform.position - Vector3.up * 0.3f, centerRay.direction);//开镜射线
            GameObject obj = Instantiate(_bullet, _muzzle.transform.position, _muzzle.transform.rotation);
            //if (_isShoulderAim && !_isAim)
            //{
            //    if (Physics.Raycast(shoulderRay, out _hitInfo, 500f))
            //    {
            //        //音效，暂时不管
            //        //GameObject fireAduio = Instantiate(_fireAudio,_muzzle.transform.position,_muzzle.transform.rotation);
            //        //AudioSource audioSource = fireAduio.GetComponent<AudioSource>();
            //        //audioSource.Play();
            //        GameObject hitObject = _hitInfo.collider.gameObject;
            //        Debug.Log("shoulder aim attack");
            //    }
            //}
            //else if (_isAim && !_isShoulderAim)
            //{
            //    if (Physics.Raycast(aimRay, out _hitInfo, 500f))
            //    {
            //        //GameObject audioObj = Instantiate(_fireAudio, _muzzlePosition.transform.position, _muzzlePosition.transform.rotation);
            //        //AudioSource audioSource = audioObj.GetComponent<AudioSource>();
            //        //audioSource.Play();
            //        GameObject hitObject = _hitInfo.collider.gameObject;
            //        Debug.Log("aim attack");
            //    }
            //}
        }
    }
}