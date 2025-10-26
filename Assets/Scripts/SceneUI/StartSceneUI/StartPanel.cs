using EventProcess;
using UIFramework.Panel;
using UnityEngine.UI;

namespace SceneUI.StartSceneUI
{
    /// <summary>
    /// 开始面板按下事件信号
    /// </summary>
    public class StartPanelPressDownSignal : ASignal{}

    /// <summary>
    /// 界面属性，可以通过实例化新的属性作为打开界面参数，以实现UI界面复用
    /// </summary>
    public class StartPanelProperties : PanelProperties
    {
        public string content;
        
        public StartPanelProperties(PanelPriority priority,string content) : base(priority)
        {
            this.content = content;
        }
    }
    
    /// <summary>
    /// 示例面板，继承PanelController额外添加自己的逻辑
    /// </summary>
    public class StartPanel : PanelController
    {
        public Text text;
        
        public void UI_OnMousePressDown()
        {
            Signals.Get<StartPanelPressDownSignal>().Dispatch();
        }

        protected override void SetProperties(IPanelProperties props)
        {
            if (props is StartPanelProperties startPanelProperties)
            {
                text.text = startPanelProperties.content;
            }
            base.SetProperties(props);
        }
    }
}