namespace Game.Player
{
    /// <summary>
    /// Базовая абстракция состояния Player Action State Machine.
    /// </summary>
    public interface IPlayerActionState
    {
        void Enter();
        void Tick();
        void Exit();
        bool CanInterrupt();
    }
}
