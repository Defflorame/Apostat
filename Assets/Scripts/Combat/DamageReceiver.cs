using UnityEngine;
using Game.Player.Stats;
namespace Game.Combat
{
       /// <summary>
    /// Общий компонент получения урона. Используется и игроком, и врагами.
    /// BlockResolver/ParryResolver/CharacterStats необязательны (может быть
    /// null) — назначаются только тем объектам, у которых они есть
    /// (у врагов до Phase 8/EnemyStats характеристик может не быть).
    /// </summary>
    [RequireComponent(typeof(HealthComponent))]
    public class DamageReceiver : MonoBehaviour
    {
        [SerializeField] private BlockResolver blockResolver;
        [SerializeField] private ParryResolver parryResolver;
        [SerializeField] private CharacterStats characterStats;

        private HealthComponent _health;

        public CharacterStats CharacterStats => characterStats;

        private void Awake()
        {
            _health = GetComponent<HealthComponent>();
        }

        /// <summary>
        /// baseDamage — уже посчитанный DamageResolver базовый урон,
        /// ДО применения блока/парирования.
        /// </summary>
        public void ReceiveDamage(DamagePacket packet, int baseDamage)
        {
            if (packet.AttackData != null && packet.AttackData.canBeParried
                && parryResolver != null && parryResolver.TryResolveIncomingAttack(packet))
            {
                Debug.Log($"{name}: атака спарирована и урон не получен.");
                return;
            }

            int finalDamage = baseDamage;

            if (packet.AttackData != null && packet.AttackData.canBeBlocked
                && blockResolver != null && blockResolver.IsBlocking)
            {
                finalDamage = blockResolver.ResolveBlockedDamage(baseDamage);
                Debug.Log($"{name}: блокировал часть урона. Должно {baseDamage} -> нанесено {finalDamage}");
            }

            if (finalDamage > 0)
            {
                _health.TakeDamage(finalDamage);
            }

            if (packet.AttackData != null && packet.AttackData.staggerPower > 0f)
            {
                ReceiveStagger(packet.AttackData.staggerPower);
            }
        }

        /// <summary>
        /// Стаггер от полученного удара. Реальная игровая реакция
        /// (прерывание анимации, StaggerState) появится вместе
        /// с Enemy AI (Phase 8) / Player Stagger (по необходимости).
        /// </summary>
        public void ReceiveStagger(float staggerPower)
        {
           // Debug.Log($"{name}: received stagger {staggerPower:F1} (пока без игровой реакции)");
        }

        /// <summary>Вызывается, когда именно ЭТОТ объект был атакующим и его атаку парировали.</summary>
        public void ReceiveParry()
        {
            Debug.Log($"{name}: ReceiveParry() — атака этого объекта была парирована.");
        }

        public void ReceiveKnockback(Vector3 direction, float force)
        {
            Debug.Log($"{name}: ReceiveKnockback({direction}, {force:F1}) — пока без физической реакции.");
        }
    }
}
