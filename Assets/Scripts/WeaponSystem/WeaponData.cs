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

using PlayerControl;
using UnityEngine;

namespace WeaponSystem
{
    public abstract class WeaponData : ScriptableObject, IWeapon
    {
        //public GameObject _player;
        //子弹与特效
        public GameObject _bullet;
        public GameObject _fireEffect;
        public GameObject _hitEffect;
        public GameObject _fireAudio;//开枪音效

        //特效位置
        //protected GameObject _rifle;//枪械位置
        protected Transform _muzzle;//枪口位置

        // 瞄准属性
        protected bool _isShoulderAim = false;
        protected bool _isAim = false;

        //射线
        protected Ray _centerRay;
        protected Ray _shoulderRay;
        protected Ray _aimRay;
        protected RaycastHit _hitInfo;//射线检测获取物体信息
        protected PlayerController _playerController;
        protected Vector3 _targetPoint;

        // 添加初始化状态标记
        protected bool _isInitialized = false;

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

        [Header("武器子弹属性")]
        [SerializeField] public int _bulletCapacity;//弹匣容量
        [SerializeField] public int _currentBulletNum;//当前枪膛里子弹数
        [SerializeField] public int _bulletTotal;//剩余总量
        [SerializeField] public float _reloadTime;//换弹时间
        public void unload(PlayerController playerController)
        {
            if (_playerController != null)
            {
                _playerController.OnAim -= setAim;
                _playerController.OnShoulderAim -= setShoulderAim;
            }

            _playerController = null;
            _muzzle = null;
            //_rifle = null;
            _isInitialized = false;
            _isAim = false;
            _isShoulderAim = false;
        }

        public void init(PlayerController playerController)
        {
            if (playerController == null)
            {
                return;
            }

            if (_isInitialized && _playerController == null)
            {
                _isInitialized = false;
            }

            if (_isInitialized && _playerController == playerController)
            {
                return;
            }

            if (_playerController != null)
            {
                _playerController.OnAim -= setAim;
                _playerController.OnShoulderAim -= setShoulderAim;
            }

            _playerController = playerController;

            _playerController.OnAim += setAim;
            _playerController.OnShoulderAim += setShoulderAim;

            FindMuzzle();

            _isInitialized = true;
        }
        protected void setShoulderAim(bool isShoulderAim)
        {
            _isShoulderAim = isShoulderAim;
        }

        protected void setAim(bool isAim)
        {
            _isAim = isAim;
        }

        protected void FindMuzzle()
        {
            GameObject rifleObj = GameObject.FindGameObjectWithTag("Rifle");
            if (rifleObj != null)
            {
                _muzzle = rifleObj.transform.Find("MuzzlePosition");
                if (_muzzle == null)
                {
                    Debug.LogError("Rifle对象上没有MuzzlePosition子物体");
                }
            }
            else
            {
                Debug.LogError("未找到Rifle对象");
            }
        }
        

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