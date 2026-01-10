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
            if (_currentBulletNum != 0)
            {
                //子弹数处理
                _currentBulletNum--;
                Debug.Log("当前子弹数： "+_currentBulletNum);
                Debug.Log("当前子弹剩余数： " + _bulletTotal);

                // 具体武器逻辑
                //射线射向屏幕中心
                _centerRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                _targetPoint = _centerRay.GetPoint(500f);

                //方向向量
                Vector3 shoulderDirection = (_targetPoint - _muzzle.transform.position).normalized;
                Vector3 aimDirection = (_targetPoint - (Camera.main.transform.position - Vector3.up * 0.3f)).normalized;

                //实例化弹道
                GameObject obj = Instantiate(_bullet, _muzzle.transform.position, _muzzle.transform.rotation);
                Bullet bullet = obj.GetComponent<Bullet>();
                bullet.setRifle(this);

                //音效
                GameObject fireAduio = Instantiate(_fireAudio, _muzzle.transform.position, _muzzle.transform.rotation);
                AudioSource audioSource = fireAduio.GetComponent<AudioSource>();
                audioSource.time = 0.34f;
                audioSource.Play();
                Destroy(fireAduio, 3);
                //目前只拿一条射线做示范
                _shoulderRay = new Ray(_muzzle.transform.position, shoulderDirection);
                if (Physics.Raycast(_shoulderRay, out _hitInfo, 500f))
                {
                    GameObject hitObject = _hitInfo.collider.gameObject;
                    if (hitObject.CompareTag("Player"))//后续根据不同物体做不同处理
                    {
                        PlayerController playerController = hitObject.GetComponent<PlayerController>();
                        playerController.Wound(bullet._rifle);
                        Debug.Log(hitObject.name + "护甲: " + playerController._armor);
                        Debug.Log(hitObject.name + "Hp: " + playerController._hp);
                    }
                }
                //暂存，之后做修改
                //if (_isShoulderAim && !_isAim)
                //{
                //    _shoulderRay = new Ray(_muzzle.transform.position, shoulderDirection);
                //    if (Physics.Raycast(_shoulderRay, out _hitInfo, 500f))
                //    {
                //        GameObject hitObject = _hitInfo.collider.gameObject;
                //        if (hitObject.CompareTag("Player")){
                //            PlayerController playerController = hitObject.GetComponent<PlayerController>();
                //            playerController.Wound(bullet._rifle);
                //            Debug.Log(hitObject.name + "护甲: " + playerController._armor);
                //            Debug.Log(hitObject.name + "Hp: " + playerController._hp);
                //        }
                //    }
                //}
                //else if (_isAim && !_isShoulderAim)
                //{
                //    _aimRay = new Ray(Camera.main.transform.position - Vector3.up * 0.3f, aimDirection);
                //    if (Physics.Raycast(_aimRay, out _hitInfo, 500f))
                //    {
                //        GameObject hitObject = _hitInfo.collider.gameObject;
                //        PlayerController playerController = hitObject.GetComponent<PlayerController>();
                //        playerController.Wound(bullet._rifle);
                //    }
                //}
            }
            else if(_currentBulletNum == 0)
            {
                return;
            }
        }
    }
}