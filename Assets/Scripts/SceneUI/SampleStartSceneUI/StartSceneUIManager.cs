using EventProcess;
using UIFramework;
using UIFramework.Panel;
using UnityEngine;

namespace SceneUI.SampleStartSceneUI
{
    /// <summary>
    /// StartScene示例UI管理器，管理当前场景的所有UI界面回调函数
    /// </summary>
    public class StartSceneUIManager : ASceneUIManager
    {
        protected override void AddSignal()
        {
            Signals.Get<StartPanelPressDownSignal>().AddListener(OnStartPanelPressDown);
        }

        protected override void RemoveSignal()
        {
            Signals.Get<StartPanelPressDownSignal>().RemoveListener(OnStartPanelPressDown);
        }
        
        // 测试使用
        void Update()
        {
            
        }
        
        #region 事件回调

        void OnStartPanelPressDown()
        {
            UIFrame.HideUI("StartPanel");
            UIFrame.ShowUI("ChatWindow");
        }
        
        #endregion
    }
}