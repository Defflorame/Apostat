using System;
using UnityEngine;

namespace Game.Combat
{
    /// <summary>
    /// Мана игрока. В отличие от StaminaComponent НЕ восстанавливается
    /// пассивно со временем: согласно п. 3.5 Пояснительной записки, ресурс
    /// маны пополняется за счёт предметов (и, позже, точек сохранения),
    /// а не тиком Update(). Поэтому здесь нет ни regen-логики, ни Update().
    ///
    /// Max задаётся извне через SetMaxMana — обычно из CharacterStats, где
    /// объём маны зависит от характеристики Intelligence.
    /// </summary>
    public class ManaComponent : MonoBehaviour
    {
        public float CurrentMana { get; private set; }
        public float MaxMana { get; private set; }

        public event Action<float, float> OnManaChanged;

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

        /// <summary>Пополнение маны предметом/точкой сохранения (сами эти системы появятся позже).</summary>
        public void Restore(float amount)
        {
            if (amount <= 0f) return;

            CurrentMana = Mathf.Min(MaxMana, CurrentMana + amount);
            OnManaChanged?.Invoke(CurrentMana, MaxMana);
        }

        /// <summary>Вызывается CharacterStats при (пере)расчёте характеристик.</summary>
        public void SetMaxMana(float newMax, bool refillToFull)
        {
            MaxMana = Mathf.Max(0f, newMax);
            CurrentMana = refillToFull ? MaxMana : Mathf.Min(CurrentMana, MaxMana);
            OnManaChanged?.Invoke(CurrentMana, MaxMana);
        }
    }
}
