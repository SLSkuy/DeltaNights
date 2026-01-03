/* ------------------------------------------------------------
 *  Author:  2023051604020 QuZhiYao
 *  Date:  2025.12.27
 *  LastUpdate:  2025.12.27
 * 
 *  功能简述：
 *  游戏局内UI面板
 * 
 * ------------------------------------------------------------ */

using EventProcess;
using UIFramework;
using UIFramework.Panel;
using UnityEngine;
using UnityEngine.UI;

namespace SceneUI.GameSceneUI
{
    /// <summary>
    /// 玩家状态面板 - 显示血条和弹药
    /// </summary>
    public class PlayerStatusPanel : PanelController
    {
        // UI组件引用
        [SerializeField] private Slider healthSlider;
        [SerializeField] private Text healthText;
        [SerializeField] private Text ammoText;
        
        // 面板属性类定义 - 继承自PanelProperties基类
        /// <summary>
        /// 角色状态面板属性
        /// </summary>
        [System.Serializable]
        public class PlayerStatusPanelProperties : PanelProperties
        {
            public float currentHealth;
            public float maxHealth;
            public int currentAmmo;
            public int maxAmmo;
            
            // 构造函数需要调用基类构造函数
            public PlayerStatusPanelProperties(
                PanelPriority priority,
                float currentHealth,
                float maxHealth,
                int currentAmmo,
                int maxAmmo) : base(priority)  // 调用基类构造函数
            {
                this.currentHealth = currentHealth;
                this.maxHealth = maxHealth;
                this.currentAmmo = currentAmmo;
                this.maxAmmo = maxAmmo;
            }
        }
        
        // 不再需要重写UIControllerID，因为基类可能会从其他地方获取
        
        /// <summary>
        /// 设置面板属性 - 使用protected访问修饰符
        /// </summary>
        protected override void SetProperties(PanelProperties props)
        {
            base.SetProperties(props);
            
            if (props is PlayerStatusPanelProperties statusProps)
            {
                UpdateHealthUI(statusProps.currentHealth, statusProps.maxHealth);
                UpdateAmmoUI(statusProps.currentAmmo, statusProps.maxAmmo);
            }
        }
        
        /// <summary>
        /// 更新血量UI
        /// </summary>
        public void UpdateHealthUI(float currentHealth, float maxHealth)
        {
            // 更新血条Slider
            if (healthSlider != null)
            {
                healthSlider.value = currentHealth / maxHealth;
            }
            
            // 更新血量文本
            if (healthText != null)
            {
                healthText.text = $"{Mathf.RoundToInt(currentHealth)}/{Mathf.RoundToInt(maxHealth)}";
            }
        }
        
        /// <summary>
        /// 更新弹药UI
        /// </summary>
        public void UpdateAmmoUI(int currentAmmo, int maxAmmo)
        {
            if (ammoText != null)
            {
                ammoText.text = $"{currentAmmo}/{maxAmmo}";
            }
        }
    }
}