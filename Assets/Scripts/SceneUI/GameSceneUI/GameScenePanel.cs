/* ------------------------------------------------------------
 *  Author:  2023051604020 QuZhiYao
 *  Date:  2025.12.27
 *  LastUpdate:  2025.12.27
 * 
 *  功能简述：
 *  游戏局内UI面板
 * 
 * ------------------------------------------------------------ */

using UnityEngine;
using UnityEngine.UI;
using UIFramework;
using UIFramework.Panel;
using EventProcess;

namespace SceneUI.GameSceneUI
{
    // 信号定义
    public class PlayerHealthChangedSignal : ASignal{}
    public class AmmoChangedSignal : ASignal{}
    public class ReloadSignal : ASignal{}
    public class DamageReceivedSignal : ASignal{}
    public class KillSignal : ASignal{}
    /// <summary>
    /// 游戏局内UI面板  
    /// </summary>
    public class GameScenePanel : PanelController
    {
        // 测试按钮
        [SerializeField] private Button healthButton;
        [SerializeField] private Button ammoButton;
        [SerializeField] private Button reloadButton;
        [SerializeField] private Button damageButton;
        [SerializeField] private Button killButton;
        public void PlayerHealthChanged()
        {
            Signals.Get<PlayerHealthChangedSignal>().Dispatch();
        }
        public void AmmoChanged()
        {
            Signals.Get<AmmoChangedSignal>().Dispatch();
        }
        public void Reload()
        {
            Signals.Get<ReloadSignal>().Dispatch();
        }
        public void DamageReceived()
        {
            Signals.Get<DamageReceivedSignal>().Dispatch();
        }
        public void Kill()
        {
            Signals.Get<KillSignal>().Dispatch();
        }
    }

}   
