/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2026.1.2
 *  LastUpdate:  2026.1.2
 *
 * 处理战局事件
 * ------------------------------------------------------------ */

using System.Collections.Generic;
using BattleSyncPackage;
using InputProcess;
using Network;
using Network.ProtoTools;
using PlayerControl;
using UnityEngine;
using UnityMath;

namespace BattleSync
{
    public class BattleManager : MonoBehaviour
    {
        [Header("战局属性")] 
        public uint roomID;
        public PlayerNetController localPlayer;
        public List<PlayerNetController> remotePlayers;
        
        [Header("战局同步频率")]
        public int sendRate = 16;
        private float _sendInterval;
        private float _timer;
        
        private void Awake()
        {
            _sendInterval = 1f / sendRate;
        }
        
        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer >= _sendInterval)
            {
                _timer -= _sendInterval;
                SendBattleSync();
            }
        }

        private void SendBattleSync()
        {
            BattleSyncRequest req = ProtoPool.NewBattleReq();
            req.PlayerID = localPlayer.playerID;
            req.RoomID = roomID;
            req.MoveDir = new Vector2D
            {
                X = GameInput.Instance.moveX.Value,
                Y = GameInput.Instance.moveZ.Value
            };
            req.Jump = GameInput.Instance.Jump > 0;

            req.Position = new Vector3D
            {
                X = localPlayer.transform.position.x,
                Y = localPlayer.transform.position.y,
                Z = localPlayer.transform.position.z
            };
            req.EulaAngle = new Vector3D
            {
                X = localPlayer.transform.eulerAngles.x,
                Y = localPlayer.transform.eulerAngles.y,
                Z = localPlayer.transform.eulerAngles.z
            };

            NetWorkManager.instance.SendUdp(req);
            req.Dispose();
        }
    }
}