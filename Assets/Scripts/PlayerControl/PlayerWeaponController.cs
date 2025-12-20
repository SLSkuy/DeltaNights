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