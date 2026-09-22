using UnityEngine;

namespace Game.Combat
{
    /// <summary>
    /// Единая система расчёта БАЗОВОГО урона для игрока и врагов.
    /// Base Damage → Final Damage. Блок и парирование НЕ здесь —
    /// это отдельная ответственность DamageReceiver/BlockResolver/ParryResolver,
    /// т.к. они зависят от состояния защищающегося, а не от формулы атаки.
    /// Сопротивления по типу урона добавятся в Phase 6 (ApplyResistance).
    /// </summary>
    public static class DamageResolver
    {
        public static void ResolveDamage(DamagePacket packet, DamageReceiver receiver)
        {
            int baseFinalDamage = CalculateFinalDamage(packet);
            receiver.ReceiveDamage(packet, baseFinalDamage);
        }

        private static int CalculateFinalDamage(DamagePacket packet)
        {
            return Mathf.RoundToInt(packet.BasePhysicalDamage * packet.DamageMultiplier);
        }
    }
}
