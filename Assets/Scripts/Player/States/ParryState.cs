using System;
using Game.Combat;

namespace Game.Player
{
    public class ParryState : IPlayerActionState
    {
        private readonly ParryResolver _parryResolver;
        private readonly Action _onFinished;

        public ParryState(ParryResolver parryResolver, Action onFinished)
        {
            _parryResolver = parryResolver;
            _onFinished = onFinished;
        }

        public void Enter()
        {
            _parryResolver.OnParryAttemptFinished += HandleFinished;
            _parryResolver.BeginParryAttempt();
        }

        public void Tick() { }

        public void Exit()
        {
            _parryResolver.OnParryAttemptFinished -= HandleFinished;
            _parryResolver.CancelParryAttempt();
        }

        public bool CanInterrupt() => false;

        private void HandleFinished()
        {
            _onFinished?.Invoke();
        }
    }
}
