using UnityEngine;
using Game.Items;

namespace Game.Combat
{
    /// <summary>
    /// Описывает отдельное воздействие оружия на цель. Не применяет урон
    /// самостоятельно — это делает DamageResolver.
    /// </summary>
    public readonly struct DamagePacket
    {
        public readonly GameObject Source;
        public readonly GameObject Target;
        public readonly int BasePhysicalDamage;
        public readonly float DamageMultiplier;
        public readonly AttackData AttackData;
        public readonly Game.Player.Stats.CharacterStats AttackerStats;

        public DamagePacket(GameObject source, GameObject target, int basePhysicalDamage, float damageMultiplier, AttackData attackData, Game.Player.Stats.CharacterStats attackerStats = null)
        {
            Source = source;
            Target = target;
            BasePhysicalDamage = basePhysicalDamage;
            DamageMultiplier = damageMultiplier;
            AttackData = attackData;
            AttackerStats = attackerStats;
        }
    }
}
