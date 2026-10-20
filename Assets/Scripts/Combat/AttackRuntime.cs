using System.Collections.Generic;
using Game.Items;

namespace Game.Combat
{
    public enum AttackPhase
    {
        Startup,
        Active,
        Recovery,
        Done
    }

    /// <summary>
    /// Состояние одной конкретной выполняемой атаки. Нужен, чтобы одна
    /// атака не наносила одной и той же цели урон более одного раза.
    /// Знает оружие, которым нанесён удар: стихия и эффекты при попадании
    /// принадлежат оружию (WeaponData), а не атаке.
    /// </summary>
    public class AttackRuntime
    {
        public AttackData AttackData { get; }
        public WeaponData Weapon { get; }
        public float StartTime { get; }
        public float DamageMultiplier { get; }
        public AttackPhase CurrentPhase { get; set; }
        public HashSet<DamageReceiver> AlreadyHitTargets { get; }

        public ElementType Element => Weapon != null ? Weapon.element : null;
        public int ElementalDamage => Weapon != null ? Weapon.GetElementalDamage(AttackData) : 0;
        public StatusEffectApplication[] OnHitEffects => Weapon != null ? Weapon.onHitEffects : null;

        public AttackRuntime(AttackData attackData, WeaponData weapon, float startTime, float damageMultiplier = 1f)
        {
            AttackData = attackData;
            Weapon = weapon;
            StartTime = startTime;
            DamageMultiplier = damageMultiplier;
            CurrentPhase = AttackPhase.Startup;
            AlreadyHitTargets = new HashSet<DamageReceiver>();
        }
    }
}
