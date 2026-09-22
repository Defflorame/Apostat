using System;
using UnityEngine;

namespace Game.Combat
{
    /// <summary>
    /// На первом этапе реализованы только получение урона и смерть.
    /// Heal/SetHealth добавятся, когда появится реальная необходимость
    /// (например, зелья или регенерация).
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
    }
}
