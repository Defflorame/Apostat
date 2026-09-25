using Game.Combat;

namespace Game.Player
{
    /// <summary>
    /// Стойка блока — активна, пока игрок держит соответствующую кнопку.
    /// В отличие от Attack, это не "committed"-действие: его можно
    /// прервать в любой момент (например, чтобы откатиться или парировать).
    /// </summary>
    public class BlockState : IPlayerActionState
    {
        private readonly BlockResolver _blockResolver;

        public BlockState(BlockResolver blockResolver)
        {
            _blockResolver = blockResolver;
        }

        public void Enter() => _blockResolver.StartBlock();

        public void Tick() { }

        public void Exit() => _blockResolver.StopBlock();

        public bool CanInterrupt() => true;
    }
}
