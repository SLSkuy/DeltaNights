using EventProcess;
using UIFramework;
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
    }
}