/* ------------------------------------------------------------
 *  Author:  2023051604032 WangXinKai
 *  Date:  2026.1.7
 *  LastUpdate:  2026.1.7
 * 
 *  功能简述：
 *  房间界面
 * 
 * ------------------------------------------------------------ */

using LobbySyncPackage;
using UIFramework.Panel;

namespace SceneUI.StartSceneUI
{
    public class RoomPanel:PanelController
    {
        public void UI_OnExitRoom()
        {
            //TODO:退出逻辑
        }

        public void UI_OnStartRoom()
        {
            //TODO:开始逻辑
        }

        //玩家加入
        public void PlayerEnter()
        {
            
        }

        //玩家退出
        public void PlayerExit()
        {
            
        }

        private void RoomEnter(RoomJoinResponsePackage response)
        {
            //TODO:处理房间信息显示
        }
        
        protected override void SetProperties(PanelProperties props)
        {
            if (props is RoomPanelProperties properties)
            {
                RoomEnter(properties.Response);
            }
            base.SetProperties(props);
        }
        
    }

    public class RoomPanelProperties : PanelProperties
    {
        public RoomJoinResponsePackage Response;

        public RoomPanelProperties(RoomJoinResponsePackage response):base(PanelPriority.None)
        {
            Response = response;
        }

    }
    
}