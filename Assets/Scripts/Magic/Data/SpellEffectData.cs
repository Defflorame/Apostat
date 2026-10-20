using UnityEngine;

namespace Game.Magic
{
    /// <summary>
    /// База для эффекта заклинания. Новый эффект — это новый наследник + ассет,
    /// SpellCaster и SpellEffectExecutor не меняются. Эффект не хранит runtime state:
    /// всё нужное приходит в SpellCastContext.
    /// </summary>
    public abstract class SpellEffectData : ScriptableObject
    {
        /// <summary>
        /// Можно ли выпустить эффект прямо сейчас (назначен префаб, есть точка прицеливания,
        /// у заклинателя есть нужный компонент). false — заклинание срывается без трат.
        /// </summary>
        public virtual bool CanExecute(SpellCastContext context) => true;

        public abstract void Execute(SpellCastContext context);
    }
}
