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
            
            ProtoBuilder.ACK.BuildConnectRequest(32);
            ProtoBuilder.ACK.BuildConnectResponse("123");
            ProtoBuilder.ACK.BuildHeartBeat(32);

            ProtoBuilder.Battle.BuildRequest(LocalBattleEvent.LocalBattleNone);
            ProtoBuilder.Battle.BuildResponse(RemoteBattleEvent.RemoteBattleNone);

            ProtoBuilder.Lobby.BuildRequest(LocalLobbyEvent.LocalLobbyNone);
            ProtoBuilder.Lobby.BuildResponse(RemoteLobbyEvent.RemoteLobbyNone);
            
            Debug.Log("ProtoTest");
        }
    }
}