using System;
using UnityEngine;
using Game.Combat;
using Game.Items;

namespace Game.Player
{
    /// <summary>
    /// Активно, пока игрок удерживает ЛКМ дольше порога тапа
    /// (см. PlayerCombatController.chargeHoldThreshold). При отпускании
    /// выполняет усиленную атаку; урон масштабируется от длительности
    /// удержания (0..maxChargeTime).
    /// </summary>
    public class ChargeAttackState : IPlayerActionState
    {
        private readonly WeaponAttackController _weaponAttackController;
        private readonly StaminaComponent _stamina;
        private readonly float _maxChargeTime;
        private readonly Action _onFinished;

        private float _chargeStartTime;
        private bool _released;

        public ChargeAttackState(WeaponAttackController weaponAttackController, StaminaComponent stamina, float maxChargeTime, Action onFinished)
        {
            _weaponAttackController = weaponAttackController;
            _stamina = stamina;
            _maxChargeTime = maxChargeTime;
            _onFinished = onFinished;
        }

        public void Enter()
        {
            _chargeStartTime = Time.time;
            _released = false;
            Debug.Log("ChargeAttackState: заряженная атака началась");
        }

        public void Tick() { }

        public void Exit() { }

        public bool CanInterrupt() => false;

        /// <summary>Вызывается PlayerCombatController при отпускании ЛКМ.</summary>
        public void NotifyReleased()
        {
            if (_released) return;
            _released = true;

            float chargeDuration = Mathf.Clamp(Time.time - _chargeStartTime, 0f, _maxChargeTime);
            float chargeRatio = _maxChargeTime > 0f ? chargeDuration / _maxChargeTime : 1f;

            AttackData heavyData = _weaponAttackController.HeavyAttackData;
            float staminaCost = heavyData != null ? heavyData.staminaCost : 0f;

            Debug.Log($"ChargeAttackState: отпустили после {chargeDuration:F2}s (ratio {chargeRatio:F2})");

            if (_stamina != null && !_stamina.TryConsume(staminaCost))
            {
                Debug.Log("ChargeAttackState: недостаточно выносливости, атака отменена");
                _onFinished?.Invoke();
                return;
            }

            _weaponAttackController.OnAttackFinished += HandleAttackFinished;
            _weaponAttackController.StartHeavyAttack(chargeRatio);
        }

        private void HandleAttackFinished()
        {
            _weaponAttackController.OnAttackFinished -= HandleAttackFinished;
            _onFinished?.Invoke();
        }
    }
}
