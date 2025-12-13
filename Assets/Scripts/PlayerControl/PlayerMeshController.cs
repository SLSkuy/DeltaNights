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
