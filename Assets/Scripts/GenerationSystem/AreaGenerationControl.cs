/* ------------------------------------------------------------
 *  Author:  2023051604032 王新凯
 *  Date:  2025.12.22
 *  LastUpdate:  2025.12.19
 *
 *  功能简述：生成系统 负责根据服务器的随机生成结果在客户端在对应位置生成物品
 * 
 * ------------------------------------------------------------ */

using System.Collections.Generic;
using UnityEngine;

namespace GenerationSystem
{
    /// <summary>
    /// 单个区域(比如单个房屋)内物品生成
    /// </summary>
    public class AreaGenerationControl: MonoBehaviour
    {
        [SerializeField]private int _areaId {get;}
        private List<LooseItem> _looseItems;
        private List<Container> _containers;
    }
}