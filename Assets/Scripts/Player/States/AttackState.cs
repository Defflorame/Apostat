using System;
using Game.Combat;

namespace Game.Player
{
    /// <summary>
    /// Состояние атаки. Пока в нём — игрок не может быть прерван
    /// (на первом этапе других действий, кроме атаки, ещё нет).
    /// </summary>
    public class AttackState : IPlayerActionState
    {
        private readonly WeaponAttackController _weaponAttackController;
        private readonly Action _onAttackFinished;

        public AttackState(WeaponAttackController weaponAttackController, Action onAttackFinished)
        {
            _weaponAttackController = weaponAttackController;
            _onAttackFinished = onAttackFinished;
        }

        public void Enter()
        {
            _weaponAttackController.OnAttackFinished += HandleAttackFinished;
            _weaponAttackController.StartLightAttack();
        }

        public void Tick() { }

        public void Exit()
        {
            _weaponAttackController.OnAttackFinished -= HandleAttackFinished;
        }

        public bool CanInterrupt() => false;

        private void HandleAttackFinished()
        {
            _onAttackFinished?.Invoke();
        }
    }
}
