using System;
using UnityEngine;
using Game.Combat;

namespace Game.Player
{
    /// <summary>
    /// Парирование: окно (ParryResolver.WindowDuration), затем восстановление.
    /// Время идёт через Tick, без корутин. При выходе из состояния окно всегда закрывается.
    /// </summary>
    public class ParryState : IPlayerActionState
    {
        private readonly ParryResolver _parryResolver;
        private readonly Action _onFinished;

        private float _elapsed;

        public ParryState(ParryResolver parryResolver, Action onFinished)
        {
            _parryResolver = parryResolver;
            _onFinished = onFinished;
        }

        public void Enter()
        {
            _elapsed = 0f;
            _parryResolver.OpenWindow();
        }

        public void Tick()
        {
            _elapsed += Time.deltaTime;

            if (_parryResolver.IsWindowOpen && _elapsed >= _parryResolver.WindowDuration)
            {
                _parryResolver.CloseWindow();
            }

            if (_elapsed >= _parryResolver.WindowDuration + _parryResolver.RecoveryDuration)
            {
                _onFinished?.Invoke();
            }
        }

        public void Exit()
        {
            _parryResolver.CloseWindow();
        }

        public bool CanInterrupt() => false;
    }
}
