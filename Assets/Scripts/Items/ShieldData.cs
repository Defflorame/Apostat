using UnityEngine;

namespace Game.Items
{
    /// <summary>
    /// Описывает щит. Не даёт бонусы характеристикам (см. IStatModifierProvider) —
    /// его эффект — blockEfficiency / magicBlockEfficiency, которые читает
    /// BlockResolver через CombatLoadout при входе в BlockState.
    /// </summary>
    [CreateAssetMenu(fileName = "NewShieldData", menuName = "Game/Items/Shield Data")]
    public class ShieldData : ItemData
    {
        [Range(0f, 1f)]
        [Tooltip("Доля физического урона, которую щит блокирует при слабой атаке.")]
        public float blockEfficiency = 0.7f;

        [Range(0f, 1f)]
        [Tooltip("Доля магического урона, которую щит блокирует при слабом заклинании. Отдельно от физического: щит может хорошо держать меч и плохо — магию.")]
        public float magicBlockEfficiency = 0.4f;

        [Header("Visual")]
        [Tooltip("Модель щита для вида от первого лица. БЕЗ коллайдеров.")]
        public GameObject prefab;

        [Tooltip("Клипы щита для левой руки: Override от LeftHand_Base (Idle, BlockRaise).")]
        public AnimatorOverrideController animatorOverride;
    }
}
