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