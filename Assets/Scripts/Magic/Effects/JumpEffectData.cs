using UnityEngine;

namespace Game.Magic
{
    /// <summary>Одноразовый высокий прыжок: подбрасывает заклинателя на height метров (работает и в воздухе).</summary>
    [CreateAssetMenu(fileName = "NewJumpEffect", menuName = "Game/Magic/Effects/High Jump")]
    public class JumpEffectData : SpellEffectData
    {
        [Tooltip("Высота подброса в метрах (обычный прыжок — около 1.2).")]
        public float height = 4f;

        public override bool CanExecute(SpellCastContext context)
        {
            return context.GetCasterComponent<ILaunchable>() != null;
        }

        public override void Execute(SpellCastContext context)
        {
            context.GetCasterComponent<ILaunchable>().Launch(height);
        }
    }
}
