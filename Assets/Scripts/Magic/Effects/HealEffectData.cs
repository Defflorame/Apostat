using UnityEngine;
using Game.Combat;

namespace Game.Magic
{
    /// <summary>Лечит заклинателя на фиксированное количество HP (не больше максимума).</summary>
    [CreateAssetMenu(fileName = "NewHealEffect", menuName = "Game/Magic/Effects/Heal")]
    public class HealEffectData : SpellEffectData
    {
        public int amount = 20;

        public override bool CanExecute(SpellCastContext context)
        {
            HealthComponent health = context.GetCasterComponent<HealthComponent>();
            return health != null && !health.IsDead();
        }

        public override void Execute(SpellCastContext context)
        {
            context.GetCasterComponent<HealthComponent>().Heal(amount);
        }
    }
}
