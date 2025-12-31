/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.29
 *  LastUpdate:  2025.12.29
 *
 *  网络玩家控制器
 *  用于映射网络玩家的所有操作到客户端角色上
 * ------------------------------------------------------------ */

using InputProcess;
using UnityEngine;

namespace PlayerControl
{
    public class PlayerNetController : MonoBehaviour
    {
        [Header("网络联机属性")] 
        public bool isLocalPlayer = true;
        public uint playerID;
        
        private PlayerController _playerController;
        private PlayerAimController _aimController;
        private PlayerAttackController _attackController;

        private void Awake()
        {
            _playerController = GetComponent<PlayerController>();
            _aimController = GetComponentInChildren<PlayerAimController>();
            _attackController = GetComponent<PlayerAttackController>();
        }

        private void Start()
        {
            Init();
        }
        
        #region 成员方法

        /// <summary>
        /// 根据是否为本地玩家初始化输入来源
        /// </summary>
        public void Init()
        {
            if (isLocalPlayer)
            {
                _playerController.Init(GameInput.Instance, isLocalPlayer);
                _aimController.Init(GameInput.Instance, _playerController, isLocalPlayer);
                _attackController.Init(GameInput.Instance);
            }
            else
            {
                // 测试使用，后续切换为网络输入源
                _playerController.Init(null, isLocalPlayer);
                _aimController.Init(null, _playerController, isLocalPlayer);
                _attackController.Init(null);
            }
        }

        #endregion
    }
}
