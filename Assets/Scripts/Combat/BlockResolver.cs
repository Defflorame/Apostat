using UnityEngine;
using Game.Items;

namespace Game.Combat
{
    /// <summary>
    /// Incoming Damage + Shield Efficiency + Attacker Power + Player Modifiers
    /// → Blocked Damage (раздел 31 документа). С Phase 4 shieldEfficiency
    /// приходит из ShieldData экипированного щита (через CombatLoadout),
    /// а не из фиксированного поля в инспекторе.
    /// </summary>
    public class BlockResolver : MonoBehaviour
    {
        [Tooltip("Урон атаки, при котором эффективность щита падает до нуля (чем сильнее удар — тем хуже блокируется).")]
        [SerializeField] private float attackerPowerSoftCap = 100f;

        private float _currentShieldEfficiency;

        public bool IsBlocking { get; private set; }

        /// <summary>shield может быть null только если блок был начат без щита — вызывающий код (PlayerCombatController) не должен этого допускать.</summary>
        public void StartBlock(ShieldData shield)
        {
            _currentShieldEfficiency = shield != null ? shield.blockEfficiency : 0f;
            IsBlocking = true;
            Debug.Log($"BlockResolver: блок начат (efficiency {_currentShieldEfficiency:F2})");
        }

        public void StopBlock()
        {
            IsBlocking = false;
            Debug.Log("BlockResolver: блок закончен");
        }

        public int ResolveBlockedDamage(int baseDamage)
        {
            if (baseDamage <= 0) return 0;

            float attackerPowerPenalty = Mathf.Clamp01(baseDamage / attackerPowerSoftCap) * _currentShieldEfficiency;
            float effectiveEfficiency = Mathf.Clamp01(_currentShieldEfficiency - attackerPowerPenalty);

            int blockedDamage = Mathf.RoundToInt(baseDamage * (1f - effectiveEfficiency));
            return Mathf.Max(0, blockedDamage);
        }
    }
}