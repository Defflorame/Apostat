using UnityEngine;

namespace Game.Combat
{
    /// <summary>
    /// Единая система расчёта БАЗОВОГО урона для игрока и врагов.
    /// Base Damage → (+ Physical Damage Bonus атакующего) → (- Defense цели) → Final Damage.
    /// Оба параметра читаются напрямую из CharacterStats через DamagePacket/DamageReceiver —
    /// путь, заложенный ещё в Phase 3 и подключённый здесь вместе с экипировкой.
    /// Block и парирование по-прежнему не здесь. Сопротивления по типу урона — Phase 6.
    /// </summary>
    public static class DamageResolver
    {
        public static void ResolveDamage(DamagePacket packet, DamageReceiver receiver)
        {
            int baseFinalDamage = CalculateFinalDamage(packet, receiver);
            receiver.ReceiveDamage(packet, baseFinalDamage);
        }


        private static int CalculateFinalDamage(DamagePacket packet, DamageReceiver receiver)
        {
            float rawDamage = packet.BasePhysicalDamage * packet.DamageMultiplier;

            float bonusPercent = packet.AttackerStats != null
                ? packet.AttackerStats.Stats.PhysicalDamageBonusPercent
                : 0f;
            rawDamage *= 1f + bonusPercent / 100f;

            float defense = receiver.CharacterStats != null
                ? receiver.CharacterStats.Stats.DefenseValue
                : 0f;

            float finalDamage = Mathf.Max(0f, rawDamage - defense);
            return Mathf.RoundToInt(finalDamage);
        }
    }
}