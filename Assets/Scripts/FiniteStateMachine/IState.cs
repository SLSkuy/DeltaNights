namespace FiniteStateMachine
{
    /// <summary>
    /// 状态机各状态接口
    /// </summary>
    public interface IState
    {
        void Enter();
        void Exit();
        void Update();
        void LateUpdate();
        void FixedUpdate();
        void OnAnimatorMove();
    }
}