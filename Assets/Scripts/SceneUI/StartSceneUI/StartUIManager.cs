/* ------------------------------------------------------------
 *  Author:  2023051604032 WangXinKai
 *  Date:  2025.12.23
 *  LastUpdate:  2025.1.3
 * 
 *  功能简述：
 *  游戏界面的UI管理器
 * 
 * ------------------------------------------------------------ */

using EventProcess;
using UIFramework;
using UIFramework.Panel;
using UIFramework.Window;
using UnityEngine;
using UnityEngine.UI;

namespace SceneUI.StartSceneUI
{
    public class StartUIManager: ASceneUIManager
    {
        protected override void AddSignal()
        {
            Signals.Get<StartPressDownSignal>().AddListener(OnStartButtonPressDown);
            Signals.Get<BackToMainPressDownSignal>().AddListener(OnBackToMainButtonPressDown);
            Signals.Get<LogInPressDownSignal>().AddListener(OnLogInButtonPressDown);
            Signals.Get<LogInPressDownErrorSignal>().AddListener(OnPromptWindow);
        }

        protected override void RemoveSignal()
        {
            Signals.Get<StartPressDownSignal>().RemoveListener(OnStartButtonPressDown);
            Signals.Get<BackToMainPressDownSignal>().RemoveListener(OnBackToMainButtonPressDown);
            Signals.Get<LogInPressDownSignal>().RemoveListener(OnLogInButtonPressDown);
            Signals.Get<LogInPressDownErrorSignal>().RemoveListener(OnPromptWindow);
        }
        
        void OnStartButtonPressDown()
        {
            UIFrame.HideUI("StartMainPanel");
            UIFrame.ShowUI("HallPanel");
        }

        void OnBackToMainButtonPressDown()
        {
            UIFrame.ShowUI("StartMainPanel");
            UIFrame.HideUI("HallPanel");
        }

        void OnLogInButtonPressDown(string ctx)
        {
            UIFrame.HideUI("LogInPanel");
            UIFrame.HideUI("PromptWindow");
            UIFrame.ShowUI("StartMainPanel",new StartMainPanelProperties(PanelPriority.None, "欢迎，"+ctx));
        }

        void OnPromptWindow(string ctx)
        {
            UIFrame.ShowUI("PromptWindow",new PromptWindowProperties(ctx));
        }

        public void TestButton()
        {
            UIFrame.ShowUI("StartMainPanel",new StartMainPanelProperties(PanelPriority.None, "启动测试"));
        }
    }
}