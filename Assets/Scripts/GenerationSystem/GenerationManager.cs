/* ------------------------------------------------------------
 *  Author:  2023051604032 王新凯
 *  Date:  2025.12.22
 *  LastUpdate:  2025.12.19
 *
 *  功能简述：生成系统 负责根据服务器的随机生成结果在客户端在对应位置生成物品
 * 
 * ------------------------------------------------------------ */

using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

namespace GenerationSystem
{
    /// <summary>
    /// 客户端生成系统总管理
    /// 根据服务器传输的生成结果生成对应物品
    /// </summary>
    public class GenerationManager: MonoBehaviour
    {
        [SerializeField]private List<AreaGenerationControl>  areas;
        [SerializeField]private string filepath;
        void Awake()
        {
            {
                //用于临时生成json
                ContainerInfo cITest=new ContainerInfo();
                cITest.ContainerId = 1;
                cITest.ItemIds=new List<int>(){1,2,3,4,10};
                
                LooseItemInfo lITest=new LooseItemInfo();
                lITest.LooseItemId = 2;
                lITest.ItemId = 11;
                
                AreaInfo aITest=new AreaInfo();
                aITest.AreaId = 3;
                aITest.Containers = new List<ContainerInfo>(){cITest};
                aITest.LooseItems=new List<LooseItemInfo>(){lITest};
                
                GenerationInfo gITest=new GenerationInfo();
                gITest.Areas=new List<AreaInfo>(){aITest};
                
                string tmp=JsonUtility.ToJson(gITest);
                File.WriteAllText(filepath,tmp);
            }
            
            string jsonstr = File.ReadAllText(filepath);
            GenerationInfo generationInfo= JsonUtility.FromJson<GenerationInfo>(jsonstr);
            AreaInfo aI=generationInfo.Areas[0];
            Debug.Log(aI);
            Debug.Log(aI.Containers[0]);
            Debug.Log(aI.LooseItems[0]);
        }
    }
}