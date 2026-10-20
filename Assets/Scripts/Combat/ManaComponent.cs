using System;
using UnityEngine;
using Game.Player.Stats;
using Game.Stats;

namespace Game.Combat
{
    /// <summary>
    /// Мана. НЕ восстанавливается пассивно (п. 3.5 пояснительной записки):
    /// пополняется предметами/точками сохранения через Restore.
    /// Максимум берётся из CharacterStats (подписка на OnStatsChanged).
    /// </summary>
    public class ManaComponent : MonoBehaviour
    {
        [Tooltip("Необязательно. Если назначено — максимум маны задаётся характеристикой Mana.")]
        [SerializeField] private CharacterStats stats;

        public float CurrentMana { get; private set; }
        public float MaxMana { get; private set; }

        public event Action<float, float> OnManaChanged;

        private void OnEnable()
        {
            if (stats != null) stats.OnStatsChanged += HandleStatsChanged;
        }

        private void OnDisable()
        {
            if (stats != null) stats.OnStatsChanged -= HandleStatsChanged;
        }

        private void Start()
        {
            if (stats != null && stats.Stats != null)
            {
                SetMaxMana(stats.Stats.MaxMana, refillToFull: true);
            }
        }

        public bool CanSpend(float amount)
        {
            return CurrentMana >= amount;
        }

        public bool TryConsume(float amount)
        {
            if (amount <= 0f) return true;

            if (!CanSpend(amount))
            {
                Debug.Log($"ManaComponent: недостаточно маны ({CurrentMana:F1}/{amount:F1})");
                return false;
            }

            CurrentMana -= amount;
            OnManaChanged?.Invoke(CurrentMana, MaxMana);
            return true;
        }

        /// <summary>Пополнение маны предметом/точкой сохранения.</summary>
        public void Restore(float amount)
        {
            if (amount <= 0f) return;

            CurrentMana = Mathf.Min(MaxMana, CurrentMana + amount);
            OnManaChanged?.Invoke(CurrentMana, MaxMana);
        }

        private void HandleStatsChanged(DerivedCharacterStats derived)
        {
            SetMaxMana(derived.MaxMana, refillToFull: false);
        }

        private void SetMaxMana(float newMax, bool refillToFull)
        {
            MaxMana = Mathf.Max(0f, newMax);
            CurrentMana = refillToFull ? MaxMana : Mathf.Min(CurrentMana, MaxMana);
            OnManaChanged?.Invoke(CurrentMana, MaxMana);
        }
    }
}
