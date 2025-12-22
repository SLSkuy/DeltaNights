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
        
        public PlayerWeaponController(PlayerAttackController attackController)
        {
            _attackController = attackController;
            
            attackController.OnAttackPressed += HandleAttackPressed;
            attackController.OnAttackReleased += HandleAttackReleased;
            attackController.OnSwitchWeapon += SwitchWeapon;
        }

        ~PlayerWeaponController()
        {
            _attackController.OnAttackPressed -= HandleAttackPressed;
            _attackController.OnAttackReleased -= HandleAttackReleased;
            _attackController.OnSwitchWeapon -= SwitchWeapon;
        }

        /// <summary>
        /// 切换武器
        /// </summary>
        /// <param name="newWeapon"></param>
        public void SwitchWeapon(WeaponData newWeapon)
        {
            _currentWeapon = newWeapon;

            _attackCooldown = 10f / _currentWeapon.AttackSpeed;
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

            _currentWeapon.Attack();

            if (_currentWeapon.AttackSpeed > 0f)
            {
                _attackCooldown = 10f / _currentWeapon.AttackSpeed;
            }
        }

        #endregion
    }
}