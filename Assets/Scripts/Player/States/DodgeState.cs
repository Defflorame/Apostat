using System;
using UnityEngine;
using Game.Player.Equipment;

namespace Game.Player
{
    /// <summary>
    /// Мгновенный отступ назад фиксированной длительности. Без окна
    /// неуязвимости — Dodge это просто отступ, а не роллинг. Дистанция
    /// отскока берётся из оружия в CombatLoadout динамически при входе
    /// в состояние (не кэшируется заранее — с Phase 4 оружие можно
    /// сменить в рантайме).
    /// </summary>
    public class DodgeState : IPlayerActionState
    {
        private readonly PlayerMovement _movement;
        private readonly CombatLoadout _combatLoadout;
        private readonly float _dodgeDuration;
        private readonly Action _onFinished;

        private float _elapsed;

        public DodgeState(PlayerMovement movement, CombatLoadout combatLoadout, float dodgeDuration, Action onFinished)
        {
            _movement = movement;
            _combatLoadout = combatLoadout;
            _dodgeDuration = dodgeDuration;
            _onFinished = onFinished;
        }

        public void Enter()
        {
            _elapsed = 0f;
            Vector3 direction = -_movement.transform.forward;

            float dodgeDistance = _combatLoadout.CurrentWeapon != null ? _combatLoadout.CurrentWeapon.dodgeDistance : 0f;
            float dodgeSpeed = _dodgeDuration > 0f ? dodgeDistance / _dodgeDuration : 0f;

            _movement.SetMovementLocked(true);
            _movement.BeginDash(direction, dodgeSpeed);
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