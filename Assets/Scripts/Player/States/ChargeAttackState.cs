using System;
using UnityEngine;
using Game.Combat;
using Game.Items;

namespace Game.Player
{
    /// <summary>
    /// Активно, пока игрок удерживает ЛКМ дольше порога тапа. При отпускании
    /// выполняет усиленную атаку. Стамина списывается через переданный делегат —
    /// состояние не знает, как именно считается цена.
    /// </summary>
    public class ChargeAttackState : IPlayerActionState
    {
        private readonly WeaponAttackController _weaponAttackController;
        private readonly Func<float, bool> _trySpendStamina;
        private readonly Action _onFinished;

        private float _chargeStartTime;
        private bool _released;

        public ChargeAttackState(WeaponAttackController weaponAttackController, Func<float, bool> trySpendStamina, Action onFinished)
        {
            _weaponAttackController = weaponAttackController;
            _trySpendStamina = trySpendStamina;
            _onFinished = onFinished;
        }

        public void Enter()
        {
            _chargeStartTime = Time.time;
            _released = false;
            Debug.Log("ChargeAttackState: заряженная атака началась");
        }

        public void Tick() { }

        public void Exit()
        {
            _weaponAttackController.OnAttackFinished -= HandleAttackFinished;
        }

        public bool CanInterrupt() => false;

        public void NotifyReleased()
        {
            if (_released) return;
            _released = true;

            AttackData heavyData = _weaponAttackController.HeavyAttackData;
            float maxChargeTime = heavyData != null ? heavyData.maxChargeDuration : 1f;

            float chargeDuration = Mathf.Clamp(Time.time - _chargeStartTime, 0f, maxChargeTime);
            float chargeRatio = maxChargeTime > 0f ? chargeDuration / maxChargeTime : 1f;
            Debug.Log($"ChargeAttackState: отпустили после {chargeDuration:F2}s (ratio {chargeRatio:F2})");

            if (!_trySpendStamina(heavyData != null ? heavyData.staminaCost : 0f))
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