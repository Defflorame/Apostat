using UnityEngine;
using Game.Player.Stats;

namespace Game.Magic
{
    /// <summary>
    /// Создаёт эффект заклинания в мире. Сейчас умеет только снаряд;
    /// новые типы эффектов добавляются здесь, а не в SpellCaster.
    /// Висит на объекте-заклинателе (он же source урона).
    /// </summary>
    public class SpellEffectExecutor : MonoBehaviour
    {
        [Tooltip("Откуда вылетает снаряд — обычно камера игрока.")]
        [SerializeField] private Transform castOrigin;
        [SerializeField] private CharacterStats characterStats;

        public void Execute(SpellData spell)
        {
            if (spell.projectilePrefab == null)
            {
                Debug.LogWarning($"SpellEffectExecutor: у {spell.name} не назначен projectilePrefab.", this);
                return;
            }

            // Временно: MagicDamage прибавляется плоско. В Phase 6 это уйдёт
            // в ElementalDamageResolver/DamageResolver вместе со стихиями.
            int magicBonus = characterStats != null ? Mathf.RoundToInt(characterStats.Stats.MagicDamage) : 0;
            int damage = spell.damage + magicBonus;

            SpellProjectile projectile = Instantiate(spell.projectilePrefab, castOrigin.position, castOrigin.rotation);
            projectile.Initialize(spell, gameObject, damage);
        }
    }
}