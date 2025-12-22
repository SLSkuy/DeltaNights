/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.14
 *  LastUpdate: 2025.12.14
 * 
 *  功能简述：
 *  ILookInputSource 定义角色视角控制输入的统一接口。
 *
 *  使用说明：
 *  - 用于向摄像机与视角控制系统提供输入数据
 *  - 通过引用方式暴露输入轴，便于第三方系统读取与修改
 *  - 设计用于本地输入与网络输入的统一抽象
 * ------------------------------------------------------------ */

using Unity.Cinemachine;

namespace InputProcess
{
    /// <summary>
    /// 视角输入接口，抽象玩家输入为数据，便于网络复用
    /// </summary>
    public interface ILookInputSource
    {
        ref InputAxis HorizontalLook { get; }
        ref InputAxis VerticalLook { get; }
    }
}