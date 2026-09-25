namespace Game.Player
{
    /// <summary>
    /// Свободное состояние: игрок не занят взаимоисключающим действием
    /// и может перейти в любое другое состояние.
    /// </summary>
    public class FreeState : IPlayerActionState
    {
        public void Enter() { }
        public void Tick() { }
        public void Exit() { }
        public bool CanInterrupt() => true;
    }
}
