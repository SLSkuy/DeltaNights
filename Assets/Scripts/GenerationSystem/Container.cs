/* ------------------------------------------------------------
 *  Author:  2023051604032 王新凯
 *  Date:  2025.12.22
 *  LastUpdate:  2025.12.19
 *
 *  功能简述：生成系统 负责根据服务器的随机生成结果在客户端在对应位置生成物品
 * 
 * ------------------------------------------------------------ */

using UnityEngine;

namespace GenerationSystem
{
    /// <summary>
    /// 容器相关操作
    /// </summary>
    public class Container: MonoBehaviour
    {
        [SerializeField]private int _containerId {get;}
        [SerializeField] private int _itemId;//临时用于检查是否收到物品id
    }
}