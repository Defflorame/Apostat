using UnityEngine;
using Game.Combat;

namespace Game.Magic
{
    /// <summary>
    /// Накладывает на заклинателя временный эффект (защита, скорость, урон и т.д.).
    /// Что именно он даёт и на сколько — описывает StatusEffectData, поэтому один и тот же
    /// эффект можно использовать в заклинаниях, предметах и атаках врагов.
    /// </summary>
    [CreateAssetMenu(fileName = "NewBuffEffect", menuName = "Game/Magic/Effects/Buff")]
    public class BuffEffectData : SpellEffectData
    {
        public StatusEffectData status;

        public override bool CanExecute(SpellCastContext context)
        {
            if (status == null)
            {
                Debug.LogWarning($"BuffEffectData '{name}': не назначен status.", this);
                return false;
            }

            StatusEffectController controller = context.GetCasterComponent<StatusEffectController>();
            return controller != null && controller.CanApply;
        }

        public override void Execute(SpellCastContext context)
        {
            context.GetCasterComponent<StatusEffectController>().Apply(status);
        }
    }
}
