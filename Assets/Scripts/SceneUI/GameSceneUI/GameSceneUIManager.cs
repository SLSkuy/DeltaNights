/* ------------------------------------------------------------
 *  Author:  2023051604020 QuZhiYao
 *  Date:  2025.12.27
 *  LastUpdate:  2025.12.27
 * 
 *  功能简述：
 *  游戏局内UI管理器
 * 
 * ------------------------------------------------------------ */


using EventProcess;
using UIFramework;
using UIFramework.Panel;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 射击游戏局内UI管理器
/// </summary>

namespace SceneUI.GameSceneUI
{
    public class GameSceneUIManager : ASceneUIManager
    {   
        protected override void AddSignal()
        {
            // 监听玩家数据变化
            Signals.Get<PlayerHealthChangedSignal>().AddListener(OnHealthChanged);// 玩家生命值变化
            Signals.Get<AmmoChangedSignal>().AddListener(OnAmmoChanged);// 弹药变化
            Signals.Get<ReloadSignal>().AddListener(OnReload);// 重新装填
            
            // 战斗相关事件
            Signals.Get<DamageReceivedSignal>().AddListener(OnDamageReceived);// 受到伤害
            Signals.Get<KillSignal>().AddListener(OnKill);// 击杀敌人
        }
        
        protected override void RemoveSignal()
        {
            //移除所有监听
            Signals.Get<PlayerHealthChangedSignal>().RemoveListener(OnHealthChanged);
            Signals.Get<AmmoChangedSignal>().RemoveListener(OnAmmoChanged);
            Signals.Get<ReloadSignal>().RemoveListener(OnReload);

            Signals.Get<DamageReceivedSignal>().RemoveListener(OnDamageReceived);
            Signals.Get<KillSignal>().RemoveListener(OnKill);
        }
        private void OnHealthChanged()
        {
            // 更新生命值UI
            Debug.Log("Player Health Changed");
        }

        private void OnAmmoChanged()
        {
            // 更新弹药UI
           Debug.Log("Ammo Changed");
        }

        private void OnReload()
        {
            // 显示重新装填UI
            Debug.Log("Reload Started");
        }

        private void OnDamageReceived()
        {
            // 显示受到伤害的UI效果
            Debug.Log("Damage Received");
        }

        private void OnKill()
        {
            // 更新击杀数UI
            Debug.Log("Kill Count Updated");
        }
    }
}