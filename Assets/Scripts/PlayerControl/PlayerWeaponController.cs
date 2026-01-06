/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.20
 *  LastUpdate:  2025.12.21
 * 
 *  功能简述：
 *  PlayerWeaponController 负责玩家武器的攻击逻辑控制，
 *  包括攻击触发、攻击冷却计算以及自动 / 半自动射击行为。
 *
 *  主要功能：
 *  - 管理当前装备的 WeaponData
 *  - 根据输入事件触发攻击行为
 *  - 处理武器攻击冷却与攻击频率限制
 *  - 支持全自动与半自动武器逻辑
 *  - 响应技能系统对攻击行为的锁定控制
 *
 *  设计说明：
 *  - 本类不直接读取输入，由 PlayerAttackController 驱动
 *  - 通过事件订阅方式解耦输入层与武器逻辑层
 *  - 不负责动画、特效或命中判定，仅触发 WeaponData.Attack
 *  - 可被 AI / 网络输入复用，具备良好的扩展性
 *
 *  使用说明：
 *  - 由 PlayerAttackController 在 Awake 中创建并持有
 *  - Tick 方法需在 Update 中持续调用
 *  - 技能释放期间可通过 SetAttackLock 禁止攻击
 * ------------------------------------------------------------ */

using System;
using UnityEngine;
using System.Diagnostics;
using WeaponSystem;

namespace PlayerControl
{
    /// <summary>
    /// 玩家武器控制器
    /// 只负责攻击逻辑 + 冷却
    /// </summary>
    public class PlayerWeaponController
    {
        private WeaponData _currentWeapon;
        private readonly PlayerAttackController _attackController;

        private float _attackCooldown;
        private bool _attackHeld;
        private bool _attackLockBySkill;
        private bool _attackLockByReloading;
        private bool _isReloading;//换弹中
        private float _reloadTimer;//换弹时间

        public event Action OnReloadComplete;

        public PlayerWeaponController(PlayerAttackController attackController)
        {
            _attackController = attackController;
            
            attackController.OnAttackPressed += HandleAttackPressed;
            attackController.OnAttackReleased += HandleAttackReleased;
            attackController.OnSwitchWeapon += SwitchWeapon;
            attackController.OnReloadStart += TryReload;
            //attackController.OnReloadComplete += CompleteReload;
        }

        ~PlayerWeaponController()
        {
            _attackController.OnAttackPressed -= HandleAttackPressed;
            _attackController.OnAttackReleased -= HandleAttackReleased;
            _attackController.OnSwitchWeapon -= SwitchWeapon;
            _attackController.OnReloadStart -= TryReload;
            //_attackController.OnReloadComplete -= CompleteReload;
        }

        /// <summary>
        /// 切换武器
        /// </summary>
        /// <param name="newWeapon"></param>
        public void SwitchWeapon(WeaponData newWeapon,PlayerController playerController)
        {
            if (newWeapon == null){return;}
            if (playerController == null){return;}
            if (_currentWeapon != null)
            {
                _currentWeapon.unload(playerController);
            }
            else {}

            _currentWeapon = newWeapon;
            _currentWeapon.init(playerController);

            if (_currentWeapon.AttackSpeed > 0f)
            {
                _attackCooldown = 10f / _currentWeapon.AttackSpeed;
            }
        }

        /// <summary>
        /// 逻辑更新
        /// </summary>
        public void Tick(float deltaTime)
        {
            if (_attackCooldown > 0f)
            {
                _attackCooldown -= deltaTime;
            }
            // 只在换弹状态下更新计时器
            if (_isReloading && _reloadTimer > 0f)
            {
                _reloadTimer -= deltaTime;
                if (_reloadTimer <= 0f)
                {
                    CompleteReload();
                }
            }

            // 全自动武器：持续攻击
            if (_attackHeld && _currentWeapon is { FullyAutomatic: true })
            {
                TryAttack();
            }
        }

        /// <summary>
        /// 设置攻击锁
        /// </summary>
        public void SetAttackLock(bool lockBySkill)
        {
            _attackLockBySkill = lockBySkill;
        }

        #region 攻击逻辑

        private void HandleAttackPressed()
        {
            _attackHeld = true;

            // 半自动：只在按下时攻击一次
            if (_currentWeapon is { FullyAutomatic: false })
            {
                TryAttack();
            }
        }

        private void HandleAttackReleased()
        {
            _attackHeld = false;
        }


        private void TryAttack()
        {
            if (!_currentWeapon) return;
            if (_attackLockBySkill) return; // 技能瞄准状态，禁止开火
            if (_attackCooldown > 0f) return;
            if (_attackLockByReloading) return;

            _currentWeapon.Attack();

            if (_currentWeapon.AttackSpeed > 0f)
            {
                _attackCooldown = 10f / _currentWeapon.AttackSpeed;
            }
        }

        #endregion

        #region 换弹逻辑
        private void TryReload()
        {
            if (_currentWeapon == null) return;
            if (_isReloading) return; // 已经在换弹中
            if (_currentWeapon._bulletTotal == 0) return;

            // 检查是否需要换弹
            if (_currentWeapon._currentBulletNum < _currentWeapon._bulletCapacity &&
                _currentWeapon._bulletTotal > 0)
            {
                StartReload();
            }
        }

        private void StartReload()
        {
            _isReloading = true;
            _reloadTimer = _currentWeapon._reloadTime;

            // 换弹期间锁定攻击
            _attackLockByReloading = true;
        }

        private void CompleteReload()
        {
            if (!_isReloading) return; 

            _isReloading = false;

            if (_currentWeapon != null)
            {
                // 计算装填子弹数
                int neededBullets = _currentWeapon._bulletCapacity - _currentWeapon._currentBulletNum;
                int availableBullets = Mathf.Min(neededBullets, _currentWeapon._bulletTotal);

                _currentWeapon._currentBulletNum += availableBullets;
                _currentWeapon._bulletTotal -= availableBullets;
            }

            _attackLockByReloading = false;
            OnReloadComplete?.Invoke();
        }
        #endregion
    }
}