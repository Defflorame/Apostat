using System.Collections.Generic;
using UnityEngine;
using Game.Magic;

namespace Game.Items
{
    /// <summary>
    /// Кольцо: бонусы характеристик (IStatModifierProvider), опциональное
    /// заклинание и префаб для отображения на пальце. Логики каста здесь нет —
    /// ей занимаются RingSpellController и SpellCaster.
    /// </summary>
    [CreateAssetMenu(fileName = "NewRingData", menuName = "Game/Items/Ring Data")]
    public class RingData : ItemData, IStatModifierProvider, IResistanceProvider
    {
        [SerializeField] private StatModifierDefinition[] statModifiers;

        [Header("Magic")]
        [Tooltip("Заклинание, которое даёт кольцо. Пусто — кольцо только со статами.")]
        [SerializeField] private SpellData spell;

        [Header("Visual")]
        [Tooltip("Модель кольца. На какой палец её надеть, решает экипировка (следующий шаг).")]
        [SerializeField] private GameObject ringPrefab;

        public IReadOnlyList<StatModifierDefinition> StatModifiers => statModifiers;

        [Header("Resistances")]
        [SerializeField] private ElementResistanceDefinition[] elementResistances;
        public IReadOnlyList<ElementResistanceDefinition> ElementResistances => elementResistances;
        public SpellData Spell => spell;
        public GameObject RingPrefab => ringPrefab;
    }
}