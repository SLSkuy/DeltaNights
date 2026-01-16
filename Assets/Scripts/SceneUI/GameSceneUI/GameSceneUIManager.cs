/* ------------------------------------------------------------
 *  Author:  2023051604020 QuZhiYao
 *  Date:  2025.12.27
 *  LastUpdate:  2025.1.4
 * 
 *  功能简述：
 *  游戏局内UI管理器
 * 
 * ------------------------------------------------------------ */


using EventProcess;
using UIFramework;
using UIFramework.Panel;
using UnityEngine;

namespace SceneUI.GameSceneUI
{
    /// <summary>
    /// 游戏场景UI管理器
    /// </summary>
    public class GameSceneUIManager : ASceneUIManager
    {
        // 信号定义
        /// <summary>
        /// 玩家血量变化信号
        /// </summary>
        public class PlayerHealthChangedSignal : ASignal<float, float> { }
        
        /// <summary>
        /// 玩家子弹数量变化信号
        /// </summary>
        public class PlayerAmmoChangedSignal : ASignal<int, int> { }
        
        // 当前UI状态
        private float currentHealth = 100f;
        private float maxHealth = 100f;
        private int currentAmmo;
        private int maxAmmo;
        
        private void Start()
        {
            // 初始化UI，显示玩家状态面板
            ShowPlayerStatusPanel();
        }
        
        /// <summary>
        /// 显示玩家状态面板
        /// </summary>
        private void ShowPlayerStatusPanel()
        {
            var props = new PlayerStatusPanel.PlayerStatusPanelProperties(
                PanelPriority.Priority,  // 使用Priority
                currentHealth,
                maxHealth,
                currentAmmo,
                maxAmmo
            );
            
            UIFrame.ShowUI("PlayerStatusPanel", props);
        }
        
        protected override void AddSignal()
        {
            // 监听玩家血量变化
            Signals.Get<PlayerHealthChangedSignal>().AddListener(OnPlayerHealthChanged);
            
            // 监听玩家弹药变化
            Signals.Get<PlayerAmmoChangedSignal>().AddListener(OnPlayerAmmoChanged);
        }
        
        protected override void RemoveSignal()
        {
            Signals.Get<PlayerHealthChangedSignal>().RemoveListener(OnPlayerHealthChanged);
            Signals.Get<PlayerAmmoChangedSignal>().RemoveListener(OnPlayerAmmoChanged);
        }
        
        #region 事件回调
        
        /// <summary>
        /// 玩家血量变化回调
        /// </summary>
        private void OnPlayerHealthChanged(float health, float max)
        {
            currentHealth = health;
            maxHealth = max;
            
            // 更新UI
            UpdatePlayerStatusUI();
        }
        
        /// <summary>
        /// 玩家弹药变化回调
        /// </summary>
        private void OnPlayerAmmoChanged(int ammo, int max)
        {
            currentAmmo = ammo;
            maxAmmo = max;
            
            // 更新UI
            UpdatePlayerStatusUI();
        }
        
        #endregion
        
        /// <summary>
        /// 更新玩家状态UI
        /// </summary>
        private void UpdatePlayerStatusUI()
        {
            var props = new PlayerStatusPanel.PlayerStatusPanelProperties(
                PanelPriority.Priority,  // 使用Priority
                currentHealth,
                maxHealth,
                currentAmmo,
                maxAmmo
            );
            
            UIFrame.ShowUI("PlayerStatusPanel", props);
        }
        
        /// <summary>
        /// 外部测试用方法：模拟玩家受伤
        /// </summary>
        public void PlayerTakeDamage(float damage)
        {
            Debug.Log("PlayerTakeDamage called");
            currentHealth = Mathf.Max(0, currentHealth - damage);//确保血量不为负
            Signals.Get<PlayerHealthChangedSignal>().Dispatch(currentHealth, maxHealth);
        }
        
        /// <summary>
        /// 外部测试用方法：模拟玩家射击
        /// </summary>
        public void PlayerFire()
        {
            Debug.Log("PlayerFire called");
            currentAmmo = Mathf.Max(0, currentAmmo - 1);//确保子弹数不为负
            Signals.Get<PlayerAmmoChangedSignal>().Dispatch(currentAmmo, maxAmmo);
        }
        
        /// <summary>
        /// 外部测试用方法：模拟玩家换弹
        /// </summary>
        public void PlayerReload()
        {
            Debug.Log("PlayerReload called");
            currentAmmo = maxAmmo;
            Signals.Get<PlayerAmmoChangedSignal>().Dispatch(currentAmmo, maxAmmo);
        }
    }
}