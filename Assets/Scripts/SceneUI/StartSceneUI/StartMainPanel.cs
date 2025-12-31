/* ------------------------------------------------------------
 *  Author:  2023051604032 WangXinKai
 *  Date:  2025.12.23
 *  LastUpdate:  2025.12.23
 * 
 *  功能简述：
 *  游戏开始界面的主界面
 * 
 * ------------------------------------------------------------ */

using EventProcess;
using TMPro;
using UIFramework;
using UIFramework.Panel;
using UnityEngine;



namespace SceneUI.StartSceneUI
{
    public class StartPressDownSignal : ASignal{}
    
    public class StartMainPanel : PanelController
    {
        [SerializeField]private TMP_Text text;
        public void UI_OnStartButtonPressDown()
        {
            Signals.Get<StartPressDownSignal>().Dispatch();
        }
        
        protected override void SetProperties(PanelProperties props)
        {
            if (props is StartMainPanelProperties startMainPanelProperties)
            {
                text.text = startMainPanelProperties.Content;
            }
            base.SetProperties(props);
        }
    }
    
    public class StartMainPanelProperties : PanelProperties
    {
        public string Content;
        
        public StartMainPanelProperties(PanelPriority priority,string content ) : base(priority)
        {
            this.Content = content;
        }
    }
}