using UnityEngine;

namespace Game.Magic
{
    /// <summary>
    /// Мгновенное попадание по лучу из точки каста вперёд (без снаряда).
    /// Промах по пустоте не срывает каст — выстрел потрачен.
    /// Пока без визуала: луч рисуется Debug.DrawLine (виден в Scene и в Game при включённых Gizmos).
    /// </summary>
    [CreateAssetMenu(fileName = "NewHitscanEffect", menuName = "Game/Magic/Effects/Hitscan")]
    public class HitscanEffectData : SpellEffectData
    {
        public int damage = 15;
        [Tooltip("Максимальная дальность луча, метры.")]
        public float range = 30f;
        [Tooltip("Может ли цель блокировать этот урон щитом.")]
        public bool canBeBlocked = true;

        public override void Execute(SpellCastContext context)
        {
            Vector3 start = context.Origin.position;

            if (!SpellAim.TryRaycast(context, range, out RaycastHit hit))
            {
                Debug.DrawLine(start, start + context.Origin.forward * range, Color.yellow, 0.5f);
                Debug.Log($"HitscanEffectData '{name}': луч никуда не попал");
                return;
            }

            Debug.DrawLine(start, hit.point, Color.red, 0.5f);
            SpellHitResolver.HitCollider(context.Spell, context.Caster, hit.collider, context.ScaleDamage(damage), canBeBlocked);
        }
    }
}
