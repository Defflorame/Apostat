using UnityEngine;

namespace Game.Combat
{
    /// <summary>
    /// Incoming Damage + Shield Efficiency + Attacker Power + Player Modifiers
    /// → Blocked Damage (раздел 31 документа).
    /// Player Modifiers (характеристики игрока) подключатся в Phase 3 —
    /// сейчас формула использует только базовую эффективность щита и силу атаки.
    /// </summary>
    public class BlockResolver : MonoBehaviour
    {
        [Range(0f, 1f)]
        [Tooltip("Доля урона, которую щит блокирует при слабой атаке.")]
        [SerializeField] private float shieldEfficiency = 0.7f;

        [Tooltip("Урон атаки, при котором эффективность щита падает до нуля (чем сильнее удар — тем хуже блокируется).")]
        [SerializeField] private float attackerPowerSoftCap = 100f;

        public bool IsBlocking { get; private set; }

        public void StartBlock()
        {
            IsBlocking = true;
            Debug.Log("BlockResolver: блок начат");
        }

        public void StopBlock()
        {
            IsBlocking = false;
            Debug.Log("BlockResolver: блок закончен");
        }

        /// <summary>
        /// Сила блока уменьшается при увеличении силы атаки противника.
        /// </summary>
        public int ResolveBlockedDamage(int baseDamage)
        {
            if (baseDamage <= 0) return 0;

            float attackerPowerPenalty = Mathf.Clamp01(baseDamage / attackerPowerSoftCap) * shieldEfficiency;
            float effectiveEfficiency = Mathf.Clamp01(shieldEfficiency - attackerPowerPenalty);

            int blockedDamage = Mathf.RoundToInt(baseDamage * (1f - effectiveEfficiency));
            return Mathf.Max(0, blockedDamage);
        }
    }
}
