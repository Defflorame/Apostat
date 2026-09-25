using System;
using UnityEngine;

namespace Game.Combat
{
    /// <summary>
    /// На первом этапе были реализованы только получение урона и смерть.
    /// В Phase 3 добавлен SetMaxHealth: теперь источником maxHealth для
    /// игрока является характеристика Health в CharacterStats, а не только
    /// константа в инспекторе. Сериализованное поле осталось значением
    /// по умолчанию — на случай объектов без CharacterStats.
    /// </summary>
    public class HealthComponent : MonoBehaviour
    {
        [SerializeField] private int maxHealth = 100;

        public int CurrentHealth { get; private set; }

        public event Action<int, int> OnHealthChanged;
        public event Action<int> OnDamageTaken;
        public event Action OnDeath;

        private bool _isDead;

        private void Awake()
        {
            CurrentHealth = maxHealth;
        }

        public void TakeDamage(int amount)
        {
            if (_isDead || amount <= 0) return;

            CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
            OnDamageTaken?.Invoke(amount);
            OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
            Debug.Log($"HealthComponent: {gameObject.name} получил {amount} урона, текущее хп {CurrentHealth}");

            if (CurrentHealth <= 0)
            {
                Kill();
            }
        }

        public void Kill()
        {
            if (_isDead) return;

            _isDead = true;
            CurrentHealth = 0;
            OnDeath?.Invoke();
        }

        public bool IsDead() => _isDead;

        /// <summary>
        /// Вызывается CharacterStats при (пере)расчёте характеристик.
        /// refillToFull=true — здоровье выставляется на новый максимум
        /// (старт игры); false — текущее здоровье лишь ограничивается новым
        /// максимумом сверху (например, при добавлении/снятии модификатора).
        /// </summary>
        public void SetMaxHealth(int newMax, bool refillToFull)
        {
            maxHealth = Mathf.Max(1, newMax);
            CurrentHealth = refillToFull ? maxHealth : Mathf.Min(CurrentHealth, maxHealth);
            OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
        }
    }
}
