using System.Collections.Generic;
using UnityEngine;

namespace Game.Items
{
    [CreateAssetMenu(fileName = "NewArmorData", menuName = "Game/Items/Armor Data")]
    public class ArmorData : ItemData, IStatModifierProvider
    {
        public EquipmentSlot slot = EquipmentSlot.Body;

        [SerializeField] private StatModifierDefinition[] statModifiers;

        public IReadOnlyList<StatModifierDefinition> StatModifiers => statModifiers;
        private void OnValidate()
        {
            if (slot == EquipmentSlot.Accessory)
            {
                Debug.LogWarning($"ArmorData '{name}': слот Accessory предназначен только для AccessoryData. Сброшено на Body.", this);
                slot = EquipmentSlot.Body;
            }
        }
    }
}