using UnityEngine;
using Game.Items;

namespace Game.Combat
{
    /// <summary>
    /// Описывает отдельное воздействие оружия на цель. Не применяет урон
    /// самостоятельно — это делает DamageResolver.
    /// Поля ElementalDamage/CanBeBlocked/CanBeParried/StaggerPower
    /// добавятся вместе с соответствующими системами (Phase 2, Phase 6).
    /// </summary>
    public readonly struct DamagePacket
    {
        public readonly GameObject Source;
        public readonly GameObject Target;
        public readonly int BasePhysicalDamage;
        public readonly float DamageMultiplier;
        public readonly AttackData AttackData;

        public DamagePacket(GameObject source, GameObject target, int basePhysicalDamage, float damageMultiplier, AttackData attackData)
        {
            Source = source;
            Target = target;
            BasePhysicalDamage = basePhysicalDamage;
            DamageMultiplier = damageMultiplier;
            AttackData = attackData;
        }
    }
}
