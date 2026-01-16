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
using UnityEngine.Animations.Rigging;

namespace PlayerControl
{
    /// <summary>
    /// 武器类型枚举
    /// </summary>
    public enum WeaponType
    {
        Rifle,
        Pistol,
    }

    /// <summary>
    /// 手持枪械位置数据类
    /// </summary>
    [System.Serializable]
    public class HoldPositionData
    {
        public GameObject weaponPrefab;  //武器预设体
        public string weaponName;        // 武器名称
        //手上
        public Vector3 positionOffset;   // 位置偏移
        public Vector3 rotationOffset;   // 旋转偏移
        public Vector3 scale; // 缩放比例
        //背部
        public Vector3 positionOffsetBack;   // 位置偏移
        public Vector3 rotationOffsetBack;   // 旋转偏移
    }

    /// <summary>
    /// 玩家模型控制器，提供模型控制接口，用于制作预制体，适配多种模型
    /// </summary>
    public class PlayerMeshController : MonoBehaviour
    {
        [Header("模型对象")]
        public GameObject playerMesh;
        public WeaponType weaponType;

        [Header("枪械信息")]
        public HoldPositionData[] weaponHoldPositions;

        private PlayerController _controller;

        private GameObject _weaponInstance;

        private Transform _rightHandTransform;
        private Transform _rightShoulderTransform;

        #region 周期函数

        private void Awake()
        {
            _controller = GetComponentInParent<PlayerController>();
        }

        private void Start()
        {
            _controller.ShowMesh += ShowMesh;

            FindGoals();
            InitWeaponPositions();
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
                }
                else if (!shouldShow && playerMesh.activeSelf)
                {
                    playerMesh.SetActive(false);
                }
            }
        }

        /// <summary>
        /// 获取当前武器的配置
        /// </summary>
        public HoldPositionData GetCurrentWeaponConfig()
        {
            int index = (int)weaponType;
            if (weaponHoldPositions != null && index >= 0 && index < weaponHoldPositions.Length)
            {
                return weaponHoldPositions[index];
            }
            return null;
        }


        /// <summary>
        /// 实例化武器预设体（初始化时在背部）
        /// </summary>
        private void InitWeaponPositions()
        {
            HoldPositionData config = GetCurrentWeaponConfig();
            if (config == null || config.weaponPrefab == null)
            {
                return;
            }

            if (_weaponInstance != null)
            {
                Destroy(_weaponInstance);
            }

            _weaponInstance = Instantiate(config.weaponPrefab, _rightShoulderTransform);
            _weaponInstance.transform.localPosition = config.positionOffsetBack;
            _weaponInstance.transform.localEulerAngles = config.rotationOffsetBack;

            if (config.scale != Vector3.zero)
            {
                _weaponInstance.transform.localScale = config.scale;
            }
        }

        private void FindGoals()
        {
            _rightHandTransform = RecursiveFindTransform(transform, "手首.R");
            _rightShoulderTransform = RecursiveFindTransform(transform, "肩.R");
        }

        /// <summary>
        /// 递归查找 Transform
        /// </summary>
        private Transform RecursiveFindTransform(Transform parent, string name)
        {
            if (parent == null) return null;

            // 如果当前节点名称匹配，直接返回
            if (parent.name == name)
                return parent;

            // 递归查找子节点
            foreach (Transform child in parent)
            {
                Transform result = RecursiveFindTransform(child, name);
                if (result != null)
                    return result;
            }

            return null;
        }

        #endregion

        #region 动画事件调用的方法

        /// <summary>
        /// 动画事件：将武器放到背上
        /// </summary>
        public void SetWeaponToBack()
        {
            if (_weaponInstance == null)
            {
                return;
            }

            HoldPositionData config = GetCurrentWeaponConfig();
            if (config == null) return;

            _weaponInstance.transform.SetParent(_rightShoulderTransform);
            _weaponInstance.transform.localPosition = config.positionOffsetBack;
            _weaponInstance.transform.localEulerAngles = config.rotationOffsetBack;
        }

        /// <summary>
        /// 动画事件：将武器拿到手上
        /// </summary>
        public void SetWeaponToHand()
        {
            if (_weaponInstance == null)
            {
                return;
            }

            HoldPositionData config = GetCurrentWeaponConfig();
            if (config == null) return;

            _weaponInstance.transform.SetParent(_rightHandTransform);
            _weaponInstance.transform.localPosition = config.positionOffset;
            _weaponInstance.transform.localEulerAngles = config.rotationOffset;
        }
        #endregion
    }
}
