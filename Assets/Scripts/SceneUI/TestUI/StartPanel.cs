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
using TMPro;
using UIFramework;
using UIFramework.Panel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace SceneUI.TextUI
{
    public class StartPressDown : ASignal{}
    
    public class StartPanel : PanelController
    {
        [SerializeField] private Button startButton;
        
         public void UI_OnStartButtonPressDown()
        {
            Signals.Get<StartPressDown>().Dispatch();
        }
    }
}