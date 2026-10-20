using UnityEngine;

namespace Game.Combat
{
    /// <summary>
    /// Возвращает HP, потраченное на заклинания с healthCostRecovers = true.
    /// Восстанавливает только «потраченную» сумму, а не лечит игрока вообще.
    /// Компонент выключен, пока восстанавливать нечего.
    /// </summary>
    public class SpentHealthRecovery : MonoBehaviour
    {
        [SerializeField] private HealthComponent health;
        [SerializeField] private float recoverPerSecond = 4f;
        [Tooltip("Пауза после траты, прежде чем HP начнёт возвращаться.")]
        [SerializeField] private float recoverDelay = 2f;

        private float _pending;
        private float _fraction;
        private float _blockedUntil;

        private void Awake()
        {
            enabled = false;
        }

        public void AddPending(int amount)
        {
            if (amount <= 0) return;

            _pending += amount;
            _blockedUntil = Time.time + recoverDelay;
            enabled = true;
        }

        private void Update()
        {
            if (health.IsDead() || _pending <= 0f)
            {
                _pending = 0f;
                _fraction = 0f;
                enabled = false;
                return;
            }

            if (Time.time < _blockedUntil) return;

            float step = Mathf.Min(recoverPerSecond * Time.deltaTime, _pending);
            _pending -= step;
            _fraction += step;

            int whole = Mathf.FloorToInt(_fraction);
            if (whole > 0)
            {
                _fraction -= whole;
                health.Heal(whole);
            }
        }
    }
}