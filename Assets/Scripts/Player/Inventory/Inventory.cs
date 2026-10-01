using System;
using System.Collections.Generic;
using UnityEngine;
using Game.Items;

namespace Game.Player
{
    /// <summary>
    /// Хранит предметы игрока. Категории (Quest/Utility/Materials, раздел 36)
    /// появятся вместе с соответствующими типами предметов — сейчас
    /// инвентарь общий для всего, что не надето (см. PlayerEquipment,
    /// куда предметы уходят/возвращаются при экипировке).
    /// </summary>
    public class Inventory : MonoBehaviour
    {
        private readonly List<ItemStack> _stacks = new List<ItemStack>();

        public event Action OnInventoryChanged;

        public void AddItem(ItemInstance item, int quantity = 1)
        {
            if (item == null || item.Data == null || quantity <= 0) return;

            if (item.Data.maxStackSize > 1)
            {
                ItemStack existingStack = FindStack(item.Data);
                if (existingStack != null)
                {
                    existingStack.Quantity = Mathf.Min(existingStack.Quantity + quantity, item.Data.maxStackSize);
                    OnInventoryChanged?.Invoke();
                    return;
                }
            }

            _stacks.Add(new ItemStack(item, quantity));
            OnInventoryChanged?.Invoke();
        }

        public bool RemoveItem(ItemInstance item, int quantity = 1)
        {
            if (item == null || quantity <= 0) return false;

            ItemStack stack = _stacks.Find(s => s.Item == item);
            if (stack == null) return false;

            stack.Quantity -= quantity;
            if (stack.Quantity <= 0)
            {
                _stacks.Remove(stack);
            }

            OnInventoryChanged?.Invoke();
            return true;
        }

        public bool HasItem(ItemData data) => FindStack(data) != null;

        public int GetItemCount(ItemData data)
        {
            ItemStack stack = FindStack(data);
            return stack != null ? stack.Quantity : 0;
        }

        public ItemStack FindItem(ItemData data) => FindStack(data);

        public IReadOnlyList<ItemStack> GetAllItems() => _stacks;

        private ItemStack FindStack(ItemData data)
        {
            return _stacks.Find(s => s.Item.Data == data);
        }
    }
}