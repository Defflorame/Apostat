using System;
using UnityEngine;

namespace Game.Player
{
    /// <summary>
    /// Мгновенный отступ назад фиксированной длительности. Без окна
    /// неуязвимости — согласно документу, Dodge это просто отступ, а не роллинг.
    /// Тайминг ведёт сам через Tick(), без корутины.
    /// </summary>
    public class DodgeState : IPlayerActionState
    {
        private readonly PlayerMovement _movement;
        private readonly float _dodgeSpeed;
        private readonly float _dodgeDuration;
        private readonly Action _onFinished;

        private float _elapsed;

        public DodgeState(PlayerMovement movement, float dodgeSpeed, float dodgeDuration, Action onFinished)
        {
            _movement = movement;
            _dodgeSpeed = dodgeSpeed;
            _dodgeDuration = dodgeDuration;
            _onFinished = onFinished;
        }

        public void Enter()
        {
            _elapsed = 0f;
            Vector3 direction = -_movement.transform.forward;

            _movement.SetMovementLocked(true);
            _movement.BeginDash(direction, _dodgeSpeed);
            Debug.Log("DodgeState: started");
        }

        public void Tick()
        {
            _elapsed += Time.deltaTime;
            if (_elapsed >= _dodgeDuration)
            {
                _onFinished?.Invoke();
            }
        }

        public void Exit()
        {
            _movement.EndDash();
            _movement.SetMovementLocked(false);
            Debug.Log("DodgeState: finished");
        }

        public bool CanInterrupt() => false;
    }
}
