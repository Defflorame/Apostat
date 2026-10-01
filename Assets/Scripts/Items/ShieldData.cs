using UnityEngine;

namespace Game.Items
{
    /// <summary>
    /// Описывает щит. Не даёт бонусы характеристикам (см. IStatModifierProvider) —
    /// его единственный эффект — blockEfficiency, который читает BlockResolver
    /// через CombatLoadout при входе в BlockState.
    /// </summary>
    [CreateAssetMenu(fileName = "NewShieldData", menuName = "Game/Items/Shield Data")]
    public class ShieldData : ItemData
    {
        [Range(0f, 1f)]
        [Tooltip("Доля урона, которую щит блокирует при слабой атаке.")]
        public float blockEfficiency = 0.7f;
    }
}