using System.Collections.Generic;
using UnityEngine;

namespace Game.Items
{
    [CreateAssetMenu(fileName = "NewAccessoryData", menuName = "Game/Items/Accessory Data")]
    public class AccessoryData : ItemData, IStatModifierProvider, IResistanceProvider
    {
        [SerializeField] private StatModifierDefinition[] statModifiers;

        public IReadOnlyList<StatModifierDefinition> StatModifiers => statModifiers;

        [Header("Resistances")]
        [SerializeField] private ElementResistanceDefinition[] elementResistances;
        public IReadOnlyList<ElementResistanceDefinition> ElementResistances => elementResistances;
    }
}