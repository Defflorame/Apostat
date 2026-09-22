using System;
using System.Collections;
using UnityEngine;
using Game.Items;

namespace Game.Combat
{
    /// <summary>
    /// Запускает атаку, читает AttackData, управляет фазами
    /// (Startup/Active/Recovery) и активацией hitbox.
    /// Корутина используется только как удобный временной процесс —
    /// логика фаз остаётся простой и предсказуемой (не подменяет state machine).
    /// </summary>
    public class WeaponAttackController : MonoBehaviour
    {
        [SerializeField] private WeaponData equippedWeapon;
        [SerializeField] private WeaponHitbox weaponHitbox;

        public event Action OnAttackFinished;

        public AttackData LightAttackData => equippedWeapon != null ? equippedWeapon.lightAttack : null;
        public AttackData HeavyAttackData => equippedWeapon != null ? equippedWeapon.heavyAttack : null;
        public WeaponData EquippedWeapon => equippedWeapon != null ? equippedWeapon : null;

        private Coroutine _attackRoutine;
        private AttackRuntime _currentAttackRuntime;

        private void Awake()
        {
            weaponHitbox.SetOwner(gameObject);
            weaponHitbox.ApplyRange(equippedWeapon.range);

        }

        public void StartLightAttack()
        {
            StartAttack(LightAttackData, 1f);
        }

        /// <summary>chargeRatio: 0 (не заряжено) .. 1 (полный заряд).</summary>
        public void StartHeavyAttack(float chargeRatio)
        {
            AttackData heavy = HeavyAttackData;
            if (heavy == null)
            {
                Debug.LogWarning("WeaponAttackController: у equippedWeapon не назначен heavyAttack.", this);
                OnAttackFinished?.Invoke();
                return;
            }

            float multiplier = Mathf.Lerp(1f, heavy.maxChargeDamageMultiplier, Mathf.Clamp01(chargeRatio));
            Debug.Log($"WeaponAttackController: heavy attack, multiplier={multiplier:F2} (chargeRatio={chargeRatio:F2})");
            StartAttack(heavy, multiplier);
        }

        public void CancelAttack()
        {
            if (_attackRoutine == null) return;

            StopCoroutine(_attackRoutine);
            _attackRoutine = null;
            DisableHitbox();
        }

        public void EnableHitbox()
        {
            weaponHitbox.Activate(_currentAttackRuntime);
        }

        public void DisableHitbox()
        {
            weaponHitbox.Deactivate();
        }

        private void StartAttack(AttackData attackData, float damageMultiplier)
        {
            if (attackData == null)
            {
                Debug.LogWarning("WeaponAttackController: attackData не назначен.", this);
                OnAttackFinished?.Invoke();
                return;
            }

            _currentAttackRuntime = new AttackRuntime(attackData, Time.time, damageMultiplier);
            _attackRoutine = StartCoroutine(RunAttackSequence(attackData));
        }

        private IEnumerator RunAttackSequence(AttackData attackData)
        {
            _currentAttackRuntime.CurrentPhase = AttackPhase.Startup;
            yield return new WaitForSeconds(attackData.startupDuration);

            _currentAttackRuntime.CurrentPhase = AttackPhase.Active;
            EnableHitbox();
            yield return new WaitForSeconds(attackData.activeDuration);

            DisableHitbox();
            _currentAttackRuntime.CurrentPhase = AttackPhase.Recovery;
            yield return new WaitForSeconds(attackData.recoveryDuration);

            _currentAttackRuntime.CurrentPhase = AttackPhase.Done;
            _attackRoutine = null;
            OnAttackFinished?.Invoke();
        }
    }
}
