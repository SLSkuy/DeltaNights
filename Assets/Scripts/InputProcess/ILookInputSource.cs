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