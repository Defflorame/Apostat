using System;
using UnityEngine;
using Game.Combat;
using Game.Items;

namespace Game.Player
{
    /// <summary>
    /// Активно, пока игрок удерживает ЛКМ дольше порога тапа
    /// (см. PlayerCombatController.chargeHoldThreshold). При отпускании
    /// выполняет усиленную атаку. MaxChargeTime и StaminaCost читаются
    /// из текущего HeavyAttackData динамически — с Phase 4 оружие можно
    /// сменить в рантайме.
    /// </summary>
    public class ChargeAttackState : IPlayerActionState
    {
        private readonly WeaponAttackController _weaponAttackController;
        private readonly StaminaComponent _stamina;
        private readonly Game.Player.Equipment.PlayerEquipment _playerEquipment;
        private readonly Action _onFinished;

        private float _chargeStartTime;
        private bool _released;

        public ChargeAttackState(WeaponAttackController weaponAttackController, StaminaComponent stamina, Game.Player.Equipment.PlayerEquipment playerEquipment, Action onFinished)
        {
            _weaponAttackController = weaponAttackController;
            _stamina = stamina;
            _playerEquipment = playerEquipment;
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

        public void NotifyReleased()
        {
            if (_released) return;
            _released = true;

            AttackData heavyData = _weaponAttackController.HeavyAttackData;
            float maxChargeTime = heavyData != null ? heavyData.maxChargeDuration : 1f;

            float chargeDuration = Mathf.Clamp(Time.time - _chargeStartTime, 0f, maxChargeTime);
            float chargeRatio = maxChargeTime > 0f ? chargeDuration / maxChargeTime : 1f;
            float staminaCost = (heavyData != null ? heavyData.staminaCost : 0f) * _playerEquipment.GetStaminaCostMultiplier();
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