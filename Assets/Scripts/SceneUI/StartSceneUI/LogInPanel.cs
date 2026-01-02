/* ------------------------------------------------------------
 *  Author:  2023051604032 WangXinKai
 *  Date:  2026.1.2
 *  LastUpdate:  2026.1.2
 * 
 *  功能简述：
 *  登录界面 目前仅有用户名/昵称
 * 
 * ------------------------------------------------------------ */

using System;
using EventProcess;
using TMPro;
using UIFramework.Panel;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace SceneUI.StartSceneUI
{
    public class LogInPressDownSignal:ASignal<string>{}
    public class LogInPressDownErrorSignal:ASignal<string>{}
    
    public class LogInPanel:PanelController
    {
        public TMP_InputField inputField;
        public void UI_OnLogInButtonPressDown()
        {
            if (inputField.text.Length > 0)
            {
                Signals.Get<LogInPressDownSignal>().Dispatch(inputField.text);
            }
            else
            {
                Signals.Get<LogInPressDownErrorSignal>().Dispatch("用户名不能为空");
            }
        }
    }
}