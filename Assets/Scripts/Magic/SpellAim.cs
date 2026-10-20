using UnityEngine;

namespace Game.Magic
{
    /// <summary>
    /// Прицеливание заклинаний: луч из точки каста (камеры) вперёд до первой поверхности.
    /// Коллайдеры самого заклинателя и триггеры игнорируются. Общий для хитскана и зон.
    /// </summary>
    public static class SpellAim
    {
        private static readonly RaycastHit[] Buffer = new RaycastHit[16];

        public static bool TryRaycast(SpellCastContext context, float maxDistance, out RaycastHit result)
        {
            result = default;
            if (context.Origin == null) return false;

            int count = Physics.RaycastNonAlloc(
                context.Origin.position,
                context.Origin.forward,
                Buffer,
                maxDistance,
                context.AimMask,
                QueryTriggerInteraction.Ignore);

            float nearest = float.MaxValue;
            bool found = false;

            for (int i = 0; i < count; i++)
            {
                RaycastHit hit = Buffer[i];
                if (SpellHitResolver.IsCaster(context.Caster, hit.collider)) continue;

                if (hit.distance < nearest)
                {
                    nearest = hit.distance;
                    result = hit;
                    found = true;
                }
            }

            return found;
        }
    }
}
