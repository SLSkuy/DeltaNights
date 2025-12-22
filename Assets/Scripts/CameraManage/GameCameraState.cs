/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.13
 *  LastUpdate: 2025.12.13
 * 
 *  功能简述：
 *  GameCameraState 用于标识游戏中摄像机的工作状态。
 *
 *  使用说明：
 *  - 用于区分不同摄像机视角模式
 *  - 作为摄像机切换与状态控制的依据
 * ------------------------------------------------------------ */

namespace CameraManage
{
    public enum GameCameraState
    {
        Normal,
        ShoulderAim,
        Aim
    }
}