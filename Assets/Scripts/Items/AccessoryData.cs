using System.Collections.Generic;
using UnityEngine;

namespace Game.Items
{
    [CreateAssetMenu(fileName = "NewAccessoryData", menuName = "Game/Items/Accessory Data")]
    public class AccessoryData : ItemData, IStatModifierProvider
    {
        [SerializeField] private StatModifierDefinition[] statModifiers;

        public IReadOnlyList<StatModifierDefinition> StatModifiers => statModifiers;
    }
}