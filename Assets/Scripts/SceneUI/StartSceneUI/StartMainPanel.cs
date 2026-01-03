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
        
        //字体自动更换
        [SerializeField] private TMP_FontAsset font;
        void OnEnable()
        {
            if (font!= null)
            {
                // 遍历所有子物体（包括自己）的TMP_Text组件
                TMP_Text[] allTexts = GetComponentsInChildren<TMP_Text>(true);
                foreach (TMP_Text text in allTexts)
                {
                    text.font = font;
                }
            }
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