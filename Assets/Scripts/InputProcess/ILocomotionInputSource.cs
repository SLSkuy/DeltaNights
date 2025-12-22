/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.14
 *  LastUpdate: 2025.12.14
 * 
 *  功能简述：
 *  ILocomotionInputSource 定义角色移动与姿态相关输入的统一接口。
 *
 *  使用说明：
 *  - 用于向移动与状态系统提供输入数据
 *  - 输入仅表示玩家意图，不直接驱动具体行为
 *  - 设计用于本地与网络输入的统一复用
 * ------------------------------------------------------------ */

namespace InputProcess
{
    /// <summary>
    /// 移动输入接口，抽象玩家输入为数据，便于网络复用
    /// </summary>
    public interface ILocomotionInputSource
    {
        float MoveX { get; }
        float MoveZ { get; }
        float Jump  { get; }
        float Aim   { get; }
        float ShoulderAim { get; }
    }
}