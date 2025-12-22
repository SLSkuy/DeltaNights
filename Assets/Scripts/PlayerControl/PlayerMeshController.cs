/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.13
 *  LastUpdate:  2025.12.13
 * 
 *  功能简述：
 *  PlayerMeshController 负责玩家模型的显示与隐藏控制，
 *  提供统一的模型管理接口，用于适配不同角色模型、
 *  第一人称 / 第三人称切换以及特殊视角表现需求。
 *
 *  主要功能：
 *  - 控制玩家模型 GameObject 的显隐状态
 *  - 响应 PlayerController 分发的模型显示事件
 *  - 解耦角色逻辑与模型表现，便于更换与复用模型
 *
 *  设计说明：
 *  - 通过监听 PlayerController.ShowMesh 事件进行驱动
 *  - 不直接参与玩家移动、攻击或输入逻辑
 *  - 模型对象以 GameObject 引用形式配置，支持多种预制体结构
 *  - 适用于第一人称开镜隐藏模型、第三人称显示模型等场景
 *
 *  使用说明：
 *  - 建议作为 PlayerController 的子物体使用
 *  - playerMesh 指向需要控制显示的模型根节点
 *  - 模型显隐逻辑统一由 PlayerController 触发，避免多处控制
 * ------------------------------------------------------------ */

using UnityEngine;

namespace PlayerControl
{
    /// <summary>
    /// 玩家模型控制器，提供模型控制接口，用于制作预制体，适配多种模型
    /// </summary>
    public class PlayerMeshController : MonoBehaviour
    {
        [Header("模型对象")] 
        public GameObject playerMesh;
        
        private PlayerController _controller;

        #region 周期函数
        
        private void Awake()
        {
            _controller = GetComponentInParent<PlayerController>();
        }

        private void Start()
        {
            _controller.ShowMesh += ShowMesh;
        }

        private void OnDestroy()
        {
            _controller.ShowMesh -= ShowMesh;
        }
        
        #endregion
        
        #region 成员方法
        
        /// <summary>
        /// 控制玩家模型显示
        /// </summary>
        private void ShowMesh(bool shouldShow)
        {
            if (playerMesh)
            {
                if (shouldShow && !playerMesh.activeSelf)
                {
                    playerMesh.SetActive(true);
                }else if (!shouldShow && playerMesh.activeSelf)
                {
                    playerMesh.SetActive(false);
                }
            }
        }
        
        #endregion
    }
}
