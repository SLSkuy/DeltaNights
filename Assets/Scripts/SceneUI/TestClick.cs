using BattleSyncPackage;
using LobbySyncPackage;
using Network;
using Network.ProtoTools;
using SyncPackage;
using UnityEngine;

namespace SceneUI
{
    public class TestClick:MonoBehaviour
    {
        //测试ProtoBuilder
        public void On_click()
        {
            
            ProtoBuilder.ClientSync.BuildConnectRequest(32);
            ProtoBuilder.ClientSync.BuildConnectResponse("123");
            ProtoBuilder.ClientSync.BuildHeartBeat(32);

            ProtoBuilder.Lobby.BuildRequest(LocalLobbyEvent.LocalLobbyNone);
            ProtoBuilder.Lobby.BuildResponse(RemoteLobbyEvent.RemoteLobbyNone);
            
            Debug.Log("ProtoTest");
        }
    }
}