using System;
using System.Collections;
using UnityEngine;
using Game.Items;
using Game.Player.Equipment;

namespace Game.Combat
{
    /// <summary>
    /// Запускает атаку, читает AttackData, управляет фазами
    /// (Startup/Active/Recovery) и активацией hitbox. С Phase 4 текущее
    /// оружие читается из CombatLoadout — оружие теперь можно сменить
    /// в рантайме через PlayerEquipment.
    /// </summary>
    public class WeaponAttackController : MonoBehaviour
    {
        [SerializeField] private CombatLoadout combatLoadout;
        [SerializeField] private WeaponHitbox weaponHitbox;

        public event Action OnAttackFinished;

        public AttackData LightAttackData => combatLoadout.CurrentWeapon != null ? combatLoadout.CurrentWeapon.lightAttack : null;
        public AttackData HeavyAttackData => combatLoadout.CurrentWeapon != null ? combatLoadout.CurrentWeapon.heavyAttack : null;
        public WeaponData EquippedWeapon => combatLoadout.CurrentWeapon;

        private Coroutine _attackRoutine;
        private AttackRuntime _currentAttackRuntime;


        private void Awake()
        {
            weaponHitbox.SetOwner(gameObject);
        }



        private void OnEnable()
        {
            combatLoadout.OnLoadoutChanged += HandleLoadoutChanged;
        }

        private void OnDisable()
        {
            combatLoadout.OnLoadoutChanged -= HandleLoadoutChanged;
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
                Debug.LogWarning("WeaponAttackController: у текущего оружия не назначен heavyAttack.", this);
                OnAttackFinished?.Invoke();
                return;
            }

            float multiplier = Mathf.Lerp(1f, heavy.maxChargeDamageMultiplier, Mathf.Clamp01(chargeRatio));
            Debug.Log($"WeaponAttackController: heavy attack, multiplier={multiplier:F2} (chargeRatio={chargeRatio:F2})");
            StartAttack(heavy, multiplier);
        }

        /// <summary>
        /// Прерывает текущую атаку (например, при смене экипировки во время
        /// Startup/Active/Recovery). Вызывает OnAttackFinished — иначе
        /// AttackState/ChargeAttackState (CanInterrupt() == false) никогда
        /// не получат сигнал вернуться в FreeState и игрок зависнет в бою.
        /// </summary>
        public void CancelAttack()
        {
            if (_attackRoutine == null) return;

            StopCoroutine(_attackRoutine);
            _attackRoutine = null;
            DisableHitbox();
            OnAttackFinished?.Invoke();
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

            float weaponRange = combatLoadout.CurrentWeapon != null ? combatLoadout.CurrentWeapon.range : 1f;
            weaponHitbox.ApplyShape(attackData.hitboxSize, weaponRange);

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

        private void HandleLoadoutChanged()
        {
            CancelAttack();
        }

    }
}