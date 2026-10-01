using System.Collections.Generic;
using UnityEngine;

namespace Game.Items
{
    /// <summary>
    /// Кольцо. В Phase 4 даёт только бонусы характеристик через
    /// IStatModifierProvider — доступ к заклинанию (RingData → SpellData)
    /// появится в Phase 5, здесь его сознательно нет.
    /// </summary>
    [CreateAssetMenu(fileName = "NewRingData", menuName = "Game/Items/Ring Data")]
    public class RingData : ItemData, IStatModifierProvider
    {
        [SerializeField] private StatModifierDefinition[] statModifiers;

        public IReadOnlyList<StatModifierDefinition> StatModifiers => statModifiers;
    }
}