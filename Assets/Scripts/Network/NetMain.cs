/* ------------------------------------------------------------
 *  Author:  2023051604046 wenrenqiang
 *  Date:  2026.1.4
 *  LastUpdate:  2026.1.4
 * 
 *  功能简述：
 *  负责初始化NetworkManger
 *
 *  主要功能：
 *  - 代替NetworkManager挂载在场景，防止场景切换后重新创建NetworkManager
 * ------------------------------------------------------------ */

using Network;
using UnityEngine;

public class NetMain : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(NetWorkManager.Instance == null)
        {
            GameObject obj = new GameObject("NetWorkManager");
            obj.AddComponent<NetWorkManager>();
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}