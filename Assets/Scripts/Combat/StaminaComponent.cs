using System;
using UnityEngine;

namespace Game.Combat
{
    /// <summary>
    /// Выносливость игрока. С Phase 3 maxStamina и regenPerSecond приходят
    /// из CharacterStats (характеристика Stamina определяет и то, и другое —
    /// см. п. 3.7 Пояснительной записки) через SetMaxStamina/SetRegenPerSecond.
    /// Сериализованные значения остались лишь значением по умолчанию —
    /// на случай объектов без CharacterStats.
    /// </summary>
    public class StaminaComponent : MonoBehaviour
    {
        [SerializeField] private float maxStamina = 100f;
        [SerializeField] private float regenPerSecond = 15f;
        [Tooltip("Пауза перед стартом регенерации после траты стамины.")]
        [SerializeField] private float regenDelayAfterConsume = 0.5f;

        public float CurrentStamina { get; private set; }

        public event Action<float, float> OnStaminaChanged;

        private bool _regenEnabled = true;
        private float _regenBlockedUntil;

        private void Awake()
        {
            CurrentStamina = maxStamina;
        }

        private void Update()
        {
            if (!_regenEnabled) return;
            if (Time.time < _regenBlockedUntil) return;
            if (CurrentStamina >= maxStamina) return;

            Restore(regenPerSecond * Time.deltaTime);
        }

        public bool CanSpend(float amount)
        {
            return CurrentStamina >= amount;
        }

        public bool TryConsume(float amount)
        {
            if (amount <= 0f) return true;

            if (!CanSpend(amount))
            {
                Debug.Log($"StaminaComponent: недостаточно stamina ({CurrentStamina:F1}/{amount:F1})");
                return false;
            }

            CurrentStamina -= amount;
            _regenBlockedUntil = Time.time + regenDelayAfterConsume;
            OnStaminaChanged?.Invoke(CurrentStamina, maxStamina);
            return true;
        }

        public void Restore(float amount)
        {
            if (amount <= 0f) return;

            CurrentStamina = Mathf.Min(maxStamina, CurrentStamina + amount);
            OnStaminaChanged?.Invoke(CurrentStamina, maxStamina);
        }

        public void StartRegeneration() => _regenEnabled = true;

        public void StopRegeneration() => _regenEnabled = false;

        /// <summary>Вызывается CharacterStats при (пере)расчёте характеристик.</summary>
        public void SetMaxStamina(float newMax, bool refillToFull)
        {
            maxStamina = Mathf.Max(0f, newMax);
            CurrentStamina = refillToFull ? maxStamina : Mathf.Min(CurrentStamina, maxStamina);
            OnStaminaChanged?.Invoke(CurrentStamina, maxStamina);
        }

        /// <summary>Вызывается CharacterStats при (пере)расчёте характеристик.</summary>
        public void SetRegenPerSecond(float value)
        {
            regenPerSecond = Mathf.Max(0f, value);
        }
    }
}
