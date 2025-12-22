/* ------------------------------------------------------------
 *  Author:  2023051604032 王新凯
 *  Date:  2025.12.22
 *  LastUpdate:  2025.12.19
 *
 *  功能简述：生成系统 负责根据服务器的随机生成结果在客户端在对应位置生成物品
 * ------------------------------------------------------------ */

using UnityEngine;
using System.Collections.Generic;

namespace GenerationSystem
{
    /// <summary>
    /// 生成结果信息
    /// </summary>
    public class GenerationInfo
    {
        public GenerationInfo(){}
        public GenerationInfo(List<AreaInfo> areas)
        {
            Areas = areas;
        }
        
        public List<AreaInfo> Areas;
    }

    [System.Serializable] public class AreaInfo
    {
        public AreaInfo(){}    
        public AreaInfo(int areaId, List<ContainerInfo> containers, List<LooseItemInfo> looseItems)
        {
            AreaId = areaId;
            Containers = containers;
            LooseItems = looseItems;
        }
        
        public int AreaId;
        public List<ContainerInfo> Containers;
        public List<LooseItemInfo> LooseItems;
    }

    [System.Serializable]public class ContainerInfo
    {
        public int ContainerId;
        public List<int> ItemIds;
    }

    [System.Serializable]public class LooseItemInfo
    {
        public int LooseItemId;
        public int ItemId;
    }
}