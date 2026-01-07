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
    //常用信号
    public class ErrorPromptWindowSignal:ASignal<string>{}//带关闭
    public class StatusPromptWindowSignal:ASignal<bool,string>{}//不带关闭
    
    public class StartUIManager: ASceneUIManager
    {
        protected override void AddSignal()
        {
            Signals.Get<StartPressDownSignal>().AddListener(OnStartButtonPressDown);
            Signals.Get<HallPressDownSignal>().AddListener(OnHallButtonPressDown);
            Signals.Get<LogInPressDownSignal>().AddListener(OnLogInButtonPressDown);
            Signals.Get<ErrorPromptWindowSignal>().AddListener(OnErrorPromptWindow);
            Signals.Get<StatusPromptWindowSignal>().AddListener(OnStatusPromptWindow);
        }

        protected override void RemoveSignal()
        {
            Signals.Get<StartPressDownSignal>().RemoveListener(OnStartButtonPressDown);
            Signals.Get<HallPressDownSignal>().RemoveListener(OnHallButtonPressDown);
            Signals.Get<LogInPressDownSignal>().RemoveListener(OnLogInButtonPressDown);
            Signals.Get<ErrorPromptWindowSignal>().RemoveListener(OnErrorPromptWindow);
            Signals.Get<StatusPromptWindowSignal>().RemoveListener(OnStatusPromptWindow);
        }
        
        void OnStartButtonPressDown()
        {
            UIFrame.HideUI("StartMainPanel");
            UIFrame.ShowUI("HallPanel");
        }

        void OnHallButtonPressDown(HallOption option)
        {
            switch (option)
            {
                case HallOption.BackToMain:
                    UIFrame.ShowUI("StartMainPanel");
                    UIFrame.HideUI("HallPanel");
                    break;
                case HallOption.CreateRoom:
                    UIFrame.ShowUI("CreateRoomWindow");
                    break;
                default:
                    Debug.Log("StartUI-HallButton:忘记改Switch了");
                    break;
            }
            
        }

        void OnLogInButtonPressDown(string ctx)
        {
            UIFrame.HideUI("LogInPanel");
            UIFrame.HideUI("StatusPromptWindow");
            UIFrame.ShowUI("StartMainPanel",new StartMainPanelProperties(PanelPriority.None, "欢迎，"+ctx));
        }

        void OnErrorPromptWindow(string ctx)
        {
            UIFrame.ShowUI("ErrorPromptWindow",new PromptWindowProperties(ctx));
        }

        void OnStatusPromptWindow(bool visible ,string ctx=null)
        {
            if (visible)
            {
                UIFrame.ShowUI("StatusPromptWindow",new PromptWindowProperties(ctx));
            }
            else
            {
                UIFrame.HideUI("StatusPromptWindow");
            }
        }

        public void TestButton()
        {
            UIFrame.ShowUI("StartMainPanel",new StartMainPanelProperties(PanelPriority.None, "启动测试"));
        }
    }
}