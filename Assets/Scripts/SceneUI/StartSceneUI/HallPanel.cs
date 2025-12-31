/* ------------------------------------------------------------
 *  Author:  2023051604032 WangXinKai
 *  Date:  2025.12.23
 *  LastUpdate:  2025.12.23
 *
 *  功能简述：
 *  游戏开始界面的大厅界面
 *
 * ------------------------------------------------------------ */

using EventProcess;
using TMPro;
using UIFramework;
using UIFramework.Panel;
using UnityEngine;


namespace SceneUI.StartSceneUI
{
    public class BackToMainPressDownSignal : ASignal{}
    public class HallPanel: PanelController
    {
        public void UI_OnBackButtonPressDown()
        {
            Signals.Get<BackToMainPressDownSignal>().Dispatch();
        }
    }
}