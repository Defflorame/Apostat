using Game.Combat;
using Game.Player.Equipment;

namespace Game.Player
{
    /// <summary>
    /// Стойка блока — активна, пока игрок держит соответствующую кнопку.
    /// С Phase 4 эффективность блока определяется щитом из CombatLoadout,
    /// а не фиксированным значением в инспекторе.
    /// </summary>
    public class BlockState : IPlayerActionState
    {
        private readonly BlockResolver _blockResolver;
        private readonly CombatLoadout _combatLoadout;

        public BlockState(BlockResolver blockResolver, CombatLoadout combatLoadout)
        {
            _blockResolver = blockResolver;
            _combatLoadout = combatLoadout;
        }

        public void Enter() => _blockResolver.StartBlock(_combatLoadout.CurrentShield);

        public void Tick() { }

        public void Exit() => _blockResolver.StopBlock();

        public bool CanInterrupt() => true;
    }
}