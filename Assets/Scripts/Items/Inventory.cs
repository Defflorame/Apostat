using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Items
{
    /// <summary>
    /// Хранит предметы игрока. Лежит в Game.Items (а не Game.Player), чтобы
    /// Equipment зависел от Items, а не от Player. Категории (Quest/Utility/Materials,
    /// раздел 36) появятся вместе с соответствующими типами предметов.
    /// Инварианты: стак никогда не больше maxStackSize; нестакающийся предмет —
    /// всегда отдельный стак из одного ItemInstance.
    /// </summary>
    public class Inventory : MonoBehaviour
    {
        private readonly List<ItemStack> _stacks = new List<ItemStack>();

        public event Action OnInventoryChanged;

        public void AddItem(ItemInstance item, int quantity = 1)
        {
            if (item == null || item.Data == null || quantity <= 0) return;

            int maxStack = Mathf.Max(1, item.Data.maxStackSize);

            // 1. Сначала дозаполняем уже существующие неполные стаки.
            if (maxStack > 1)
            {
                foreach (ItemStack stack in _stacks)
                {
                    if (stack.Item.Data != item.Data || stack.Quantity >= maxStack) continue;

                    int toAdd = Mathf.Min(maxStack - stack.Quantity, quantity);
                    stack.Quantity += toAdd;
                    quantity -= toAdd;

                    if (quantity <= 0) break;
                }
            }

            // 2. Остаток — новые стаки не больше maxStack. Для нестакающихся предметов
            //    (maxStack == 1) каждая единица получает собственный ItemInstance:
            //    у экземпляров будет своё состояние (уровень заточки и т.д.).
            bool isFirstNewStack = true;
            while (quantity > 0)
            {
                int amount = Mathf.Min(maxStack, quantity);
                ItemInstance instance = isFirstNewStack ? item : new ItemInstance(item.Data);

                _stacks.Add(new ItemStack(instance, amount));
                quantity -= amount;
                isFirstNewStack = false;
            }

            OnInventoryChanged?.Invoke();
        }

        /// <summary>
        /// Нестакающийся предмет: убирает именно этот экземпляр (quantity должно быть 1).
        /// Стакающийся: убирает quantity штук из любых стаков этого типа.
        /// false — ничего не изменено (предмета нет или его недостаточно).
        /// </summary>
        public bool RemoveItem(ItemInstance item, int quantity = 1)
        {
            if (item == null || item.Data == null || quantity <= 0) return false;

            if (item.Data.maxStackSize <= 1)
            {
                if (quantity != 1) return false;

                int index = _stacks.FindIndex(s => s.Item == item);
                if (index < 0) return false;

                _stacks.RemoveAt(index);
                OnInventoryChanged?.Invoke();
                return true;
            }

            if (GetItemCount(item.Data) < quantity) return false;

            for (int i = _stacks.Count - 1; i >= 0 && quantity > 0; i--)
            {
                ItemStack stack = _stacks[i];
                if (stack.Item.Data != item.Data) continue;

                int taken = Mathf.Min(stack.Quantity, quantity);
                stack.Quantity -= taken;
                quantity -= taken;

                if (stack.Quantity <= 0)
                {
                    _stacks.RemoveAt(i);
                }
            }

            OnInventoryChanged?.Invoke();
            return true;
        }

        public bool HasItem(ItemData data) => GetItemCount(data) > 0;

        /// <summary>Суммарное количество по ВСЕМ стакам этого типа предмета.</summary>
        public int GetItemCount(ItemData data)
        {
            int total = 0;
            foreach (ItemStack stack in _stacks)
            {
                if (stack.Item.Data == data) total += stack.Quantity;
            }

            return total;
        }

        public ItemStack FindItem(ItemData data) => _stacks.Find(s => s.Item.Data == data);

        public IReadOnlyList<ItemStack> GetAllItems() => _stacks;
    }
}
