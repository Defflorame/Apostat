using System.Collections.Generic;
using UnityEngine;

namespace Game.Items
{
    [CreateAssetMenu(fileName = "NewArmorData", menuName = "Game/Items/Armor Data")]
    public class ArmorData : ItemData, IStatModifierProvider
    {
        public EquipmentSlot slot = EquipmentSlot.Body;

        [SerializeField] private StatModifierDefinition[] statModifiers;

        [Header("Visual")]
        [Tooltip("Модель руки в этих перчатках. Используется ТОЛЬКО при slot = Gloves. Пусто — рука остаётся голой.")]
        public GameObject handPrefab;

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