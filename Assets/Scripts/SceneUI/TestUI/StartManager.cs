/* ------------------------------------------------------------
 *  Author:  2023051604020 QuZhiYao
 *  Date:  2025.12.24
 *  LastUpdate:  2025.12.24
 * 
 *  功能简述：
 *  游戏开始界面的UI管理器
 * 
 * ------------------------------------------------------------ */

using EventProcess;
using UIFramework;
using UIFramework.Panel;
using UnityEngine;
using UnityEngine.UI;

namespace SceneUI.TextUI
{
    public class StartManager: ASceneUIManager
    {
        protected override void AddSignal()
        {
            Signals.Get<StartPressDown>().AddListener(StartButtonPressDown); 
        }

        protected override void RemoveSignal()
        {
            Signals.Get<StartPressDown>().RemoveListener(StartButtonPressDown);
        }   

        void StartButtonPressDown()
        {
            UIFrame.HideUI("StartPanel");
            UIFrame.ShowUI("LobbyPanel");
        } 
        public void TestButton()
        {
            UIFrame.ShowUI("StartMainPanel",new StartMainPanelProperties(PanelPriority.None, "启动测试"));
        }
        
    }
}