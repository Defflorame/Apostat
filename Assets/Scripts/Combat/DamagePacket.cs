using UnityEngine;
using Game.Items;

namespace Game.Combat
{
    /// <summary>
    /// Описывает отдельное воздействие на цель. Не применяет урон
    /// самостоятельно — это делает DamageResolver.
    /// Урон разделён на две части: BasePhysicalDamage (физика) и ElementalDamage
    /// со стихией Element (null = нейтральный). Заклинания (Kind = Magic) кладут весь
    /// урон в ElementalDamage, оружие — физику в BasePhysicalDamage и, при желании,
    /// стихийную добавку в ElementalDamage. OnHitEffects — статус-эффекты, которые
    /// пакет пытается наложить, если по цели прошёл урон.
    /// </summary>
    public readonly struct DamagePacket
    {
        public readonly GameObject Source;
        public readonly GameObject Target;
        public readonly int BasePhysicalDamage;
        public readonly float DamageMultiplier;
        public readonly AttackData AttackData;
        public readonly Stats.CharacterStats AttackerStats;
        public readonly DamageKind Kind;
        public readonly bool CanBeBlocked;
        public readonly ElementType Element;
        public readonly int ElementalDamage;
        public readonly StatusEffectApplication[] OnHitEffects;
        public readonly bool IgnoreDefense;

        public DamagePacket(
            GameObject source,
            GameObject target,
            int basePhysicalDamage,
            float damageMultiplier,
            AttackData attackData,
            Stats.CharacterStats attackerStats = null,
            DamageKind kind = DamageKind.Physical,
            bool canBeBlocked = true,
            ElementType element = null,
            int elementalDamage = 0,
            StatusEffectApplication[] onHitEffects = null,
            bool ignoreDefense = false)
        {
            Source = source;
            Target = target;
            BasePhysicalDamage = basePhysicalDamage;
            DamageMultiplier = damageMultiplier;
            AttackData = attackData;
            AttackerStats = attackerStats;
            Kind = kind;
            CanBeBlocked = canBeBlocked;
            Element = element;
            ElementalDamage = elementalDamage;
            OnHitEffects = onHitEffects;
            IgnoreDefense = ignoreDefense;

        }

        /// <summary>
        /// Атаки с AttackData (оружие) берут блокируемость из него,
        /// остальные (заклинания) — из собственного поля CanBeBlocked.
        /// </summary>
        public bool IsBlockable => AttackData != null ? AttackData.canBeBlocked : CanBeBlocked;
    }
}
