/* ------------------------------------------------------------
 *  Author:  2023051604032 WangXinKai
 *  Date:  2025.12.23
 *  LastUpdate:  2025.12.23
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

namespace SceneUI.StartSceneUI
{
    public class StartUIManager: ASceneUIManager
    {
        protected override void AddSignal()
        {
            Signals.Get<StartPressDownSignal>().AddListener(OnStartButtonPressDown);
        }

        protected override void RemoveSignal()
        {
            Signals.Get<StartPressDownSignal>().RemoveListener(OnStartButtonPressDown);
        }
        
        void OnStartButtonPressDown()
        {
            UIFrame.HideUI("StartMainPanel");

            Debug.Log("StartMainPanelHide");
        }

        public void TestButton()
        {
            UIFrame.ShowUI("StartMainPanel",new StartMainPanelProperties(PanelPriority.None, "启动测试"));
        }
    }
}