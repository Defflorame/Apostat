using UnityEngine;

namespace Game.Magic
{
    /// <summary>
    /// Эффект «выпустить снаряд». explosionRadius > 0 превращает его в снаряд со взрывом:
    /// при попадании урон получают все цели в радиусе (каждая один раз).
    /// </summary>
    [CreateAssetMenu(fileName = "NewProjectileEffect", menuName = "Game/Magic/Effects/Projectile")]
    public class ProjectileEffectData : SpellEffectData
    {
        public SpellProjectile projectilePrefab;
        public int damage = 20;
        public float speed = 20f;
        public float lifetime = 3f;

        [Header("Explosion")]
        [Tooltip("0 — урон только той цели, в которую попал снаряд. > 0 — взрыв с этим радиусом (метры) в точке попадания, в том числе при попадании в стену.")]
        public float explosionRadius = 0f;
        [Tooltip("Бьёт ли взрыв по самому заклинателю.")]
        public bool explosionAffectsCaster = true;

        [Header("Defense")]
        [Tooltip("Может ли цель блокировать этот урон щитом.")]
        public bool canBeBlocked = false;

        public override bool CanExecute(SpellCastContext context)
        {
            if (projectilePrefab == null)
            {
                Debug.LogWarning($"ProjectileEffectData '{name}': не назначен projectilePrefab.", this);
                return false;
            }

            return true;
        }

        public override void Execute(SpellCastContext context)
        {
            SpellProjectile projectile = Instantiate(projectilePrefab, context.Origin.position, context.Origin.rotation);
            projectile.Initialize(this, context, context.ScaleDamage(damage));
        }
    }
}
