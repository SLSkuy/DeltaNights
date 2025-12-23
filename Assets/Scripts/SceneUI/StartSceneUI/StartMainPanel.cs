using EventProcess;
using UIFramework;
using UIFramework.Panel;
using UnityEngine.UI;


namespace SceneUI.StartSceneUI
{
    public class StartPressDownSignal : ASignal{}
    
    public class StartMainPanel : PanelController
    {
        public void UI_OnStartButtonPressDown()
        {
            Signals.Get<StartPressDownSignal>().Dispatch();
        }
    }
}