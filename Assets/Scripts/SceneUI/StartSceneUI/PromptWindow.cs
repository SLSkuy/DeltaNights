/* ------------------------------------------------------------
 *  Author:  2023051604032 WangXinKai
 *  Date:  2026.1.2
 *  LastUpdate:  2026.1.2
 * 
 *  功能简述：
 *  提示信息或报错信息显示
 * 
 * ------------------------------------------------------------ */

using TMPro;
using UIFramework.Panel;
using UIFramework.Window;
using UnityEngine;

namespace SceneUI.StartSceneUI
{
    public class PromptWindow:WindowController
    {
        [SerializeField]private TMP_Text text;
        
        protected override void SetProperties(WindowProperties props)
        {
            if (props is PromptWindowProperties promptWindowProperties)
            {
                text.text = promptWindowProperties.Content;
            }
            base.SetProperties(props);
        }
    }

    public class PromptWindowProperties:WindowProperties
    {
        public string Content;

        public PromptWindowProperties(string content, WindowPriority properties=WindowPriority.ForceForeground,
            bool hideOnForegroundLost=false,bool isPopup=false) :base(properties,hideOnForegroundLost,isPopup)
        {
            Content = content;
        }
    }
}