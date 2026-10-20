using UnityEngine;

namespace Game.Magic
{
    /// <summary>Всё, что нужно эффекту заклинания в момент выпуска. Только данные.</summary>
    public readonly struct SpellCastContext
    {
        public readonly SpellData Spell;
        public readonly GameObject Caster;
        public readonly Transform Origin;
        public readonly float MagicDamage;
        public readonly LayerMask AimMask;

        public SpellCastContext(SpellData spell, GameObject caster, Transform origin, float magicDamage, LayerMask aimMask)
        {
            Spell = spell;
            Caster = caster;
            Origin = origin;
            MagicDamage = magicDamage;
            AimMask = aimMask;
        }

        /// <summary>
        /// Временно: MagicDamage заклинателя прибавляется к урону плоско.
        /// В Phase 6 это уйдёт в DamageResolver вместе со стихиями — менять нужно будет только здесь.
        /// </summary>
        public int ScaleDamage(int baseDamage)
        {
            return baseDamage + Mathf.RoundToInt(MagicDamage);
        }

        /// <summary>Компонент заклинателя (например, HealthComponent). Вызывается только в момент каста.</summary>
        public T GetCasterComponent<T>() where T : class
        {
            return Caster != null ? Caster.GetComponentInChildren<T>() : null;
        }
    }
}
