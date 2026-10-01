using System.Collections.Generic;
using Game.Items;
using Game.Player.Stats;

namespace Game.Player.Equipment
{
    /// <summary>
    /// Stateless-сервис: переводит бонусы предмета (IStatModifierProvider)
    /// в StatModifier на CharacterStats и обратно, и считает суммарный вес
    /// экипированных предметов для EncumbranceService. Не хранит состояние —
    /// PlayerEquipment/HandEquipment остаются единственным источником истины
    /// о том, что сейчас надето.
    /// </summary>
    public static class EquipmentModifierService
    {
        public static void ApplyItemModifiers(CharacterStats stats, ItemInstance item)
        {
            if (stats == null || item?.Data == null) return;
            if (!(item.Data is IStatModifierProvider provider)) return;

            foreach (StatModifierDefinition definition in provider.StatModifiers)
            {
                stats.AddModifier(new StatModifier(definition.stat, definition.type, definition.value, item));
            }
        }

        public static void RemoveItemModifiers(CharacterStats stats, ItemInstance item)
        {
            if (stats == null || item == null) return;
            stats.RemoveModifiersFromSource(item);
        }

        public static float GetTotalWeight(IEnumerable<ItemInstance> slotItems, IEnumerable<ItemInstance> rings, ItemInstance weapon, ItemInstance shield)
        {
            float total = 0f;

            foreach (ItemInstance item in slotItems)
            {
                total += ItemWeight(item);
            }

            foreach (ItemInstance item in rings)
            {
                total += ItemWeight(item);
            }

            total += ItemWeight(weapon);
            total += ItemWeight(shield);

            return total;
        }

        private static float ItemWeight(ItemInstance item)
        {
            return item?.Data != null ? item.Data.weight : 0f;
        }
    }
}