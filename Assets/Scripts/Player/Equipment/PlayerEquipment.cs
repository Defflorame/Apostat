using System;
using System.Collections.Generic;
using UnityEngine;
using Game.Items;
using Game.Player.Stats;

namespace Game.Player.Equipment
{
    /// <summary>
    /// Координатор экипировки игрока: слоты брони/аксессуара, кольца
    /// (список без жёсткого enum-слота на каждое — раздел 37: "количество
    /// колец не должно быть захардкожено в Combat System") и делегирование
    /// рук в HandEquipment. Обмен предметами с Inventory и применение
    /// StatModifier через EquipmentModifierService происходит здесь —
    /// HandEquipment и Inventory сами об этом не знают.
    /// </summary>
    public class PlayerEquipment : MonoBehaviour
    {
        [SerializeField] private HandEquipment handEquipment;
        [SerializeField] private Inventory inventory;
        [SerializeField] private CharacterStats characterStats;

        [Tooltip("Максимальное количество одновременно надетых колец.")]
        [SerializeField] private int maxRingSlots = 2;

        private readonly Dictionary<EquipmentSlot, ItemInstance> _slots = new Dictionary<EquipmentSlot, ItemInstance>();
        private readonly List<ItemInstance> _equippedRings = new List<ItemInstance>();

        public event Action OnEquipmentChanged;

        public HandEquipment Hands => handEquipment;
        public IReadOnlyList<ItemInstance> EquippedRings => _equippedRings;

        public ItemInstance GetEquipped(EquipmentSlot slot)
        {
            return _slots.TryGetValue(slot, out ItemInstance item) ? item : null;
        }

        public bool Equip(ItemInstance item, EquipmentSlot slot)
        {
            if (!CanEquipToSlot(item, slot))
            {
                Debug.LogWarning($"PlayerEquipment: {item?.Data?.itemName} нельзя экипировать в слот {slot}.", this);
                return false;
            }

            UnequipInternal(slot);
            inventory.RemoveItem(item);

            _slots[slot] = item;
            EquipmentModifierService.ApplyItemModifiers(characterStats, item);
            OnEquipmentChanged?.Invoke();
            return true;
        }

        public ItemInstance Unequip(EquipmentSlot slot)
        {
            ItemInstance item = UnequipInternal(slot);
            if (item != null)
            {
                OnEquipmentChanged?.Invoke();
            }

            return item;
        }

        public bool EquipRing(ItemInstance item)
        {
            if (item == null || !(item.Data is RingData))
            {
                Debug.LogWarning($"PlayerEquipment: {item?.Data?.itemName} не является кольцом.", this);
                return false;
            }

            if (_equippedRings.Count >= maxRingSlots)
            {
                Debug.Log("PlayerEquipment: нет свободных слотов для колец.");
                return false;
            }

            inventory.RemoveItem(item);
            _equippedRings.Add(item);
            EquipmentModifierService.ApplyItemModifiers(characterStats, item);
            OnEquipmentChanged?.Invoke();
            return true;
        }

        public bool UnequipRing(ItemInstance item)
        {
            if (item == null || !_equippedRings.Remove(item)) return false;

            EquipmentModifierService.RemoveItemModifiers(characterStats, item);
            inventory.AddItem(item);
            OnEquipmentChanged?.Invoke();
            return true;
        }

        /// <summary>Экипирует оружие в MainHand. Если оружие двуручное — освобождает OffHand.</summary>
        public bool EquipMainHandWeapon(ItemInstance item)
        {
            if (item == null || !(item.Data is WeaponData))
            {
                Debug.LogWarning($"PlayerEquipment: {item?.Data?.itemName} не является оружием.", this);
                return false;
            }

            inventory.RemoveItem(item);
            bool success = handEquipment.EquipMainHandWeapon(item, out ItemInstance previousMainHand, out ItemInstance displacedOffHand);

            if (!success)
            {
                inventory.AddItem(item);
                return false;
            }

            if (previousMainHand != null)
            {
                inventory.AddItem(previousMainHand);
            }

            if (displacedOffHand != null)
            {
                inventory.AddItem(displacedOffHand);
            }

            OnEquipmentChanged?.Invoke();
            return true;
        }

        public void UnequipMainHandWeapon()
        {
            ItemInstance previous = handEquipment.UnequipMainHand();
            if (previous != null)
            {
                inventory.AddItem(previous);
                OnEquipmentChanged?.Invoke();
            }
        }

        /// <summary>Второе одноручное оружие в OffHand (dual-wield).</summary>
        public bool EquipOffHandWeapon(ItemInstance item)
        {
            if (item == null || !(item.Data is WeaponData))
            {
                Debug.LogWarning($"PlayerEquipment: {item?.Data?.itemName} не является оружием.", this);
                return false;
            }

            inventory.RemoveItem(item);
            bool success = handEquipment.EquipOffHandWeapon(item, out ItemInstance displaced);

            if (!success)
            {
                inventory.AddItem(item);
                return false;
            }

            if (displaced != null)
            {
                inventory.AddItem(displaced);
            }

            OnEquipmentChanged?.Invoke();
            return true;
        }

        public bool EquipShield(ItemInstance item)
        {
            if (item == null || !(item.Data is ShieldData))
            {
                Debug.LogWarning($"PlayerEquipment: {item?.Data?.itemName} не является щитом.", this);
                return false;
            }

            inventory.RemoveItem(item);
            bool success = handEquipment.EquipShield(item, out ItemInstance displaced);

            if (!success)
            {
                inventory.AddItem(item);
                return false;
            }

            if (displaced != null)
            {
                inventory.AddItem(displaced);
            }

            OnEquipmentChanged?.Invoke();
            return true;
        }

        /// <summary>Снимает то, что сейчас в OffHand (щит или второе оружие).</summary>
        public void UnequipOffHand()
        {
            ItemInstance previous = handEquipment.UnequipOffHand();
            if (previous != null)
            {
                inventory.AddItem(previous);
                OnEquipmentChanged?.Invoke();
            }
        }

        public float GetTotalWeight()
        {
            return EquipmentModifierService.GetTotalWeight(_slots.Values, _equippedRings, handEquipment.MainHandItem, handEquipment.OffHandItem);
        }

        /// <summary>Множитель стоимости стамины от текущей нагрузки экипировки — читает Combat (PlayerCombatController/ChargeAttackState) перед списанием стамины за действие.</summary>
        public float GetStaminaCostMultiplier()
        {
            float loadRatio = EncumbranceService.GetLoadRatio(GetTotalWeight(), characterStats.Stats.CarryWeightLimit);
            return EncumbranceService.GetStaminaCostMultiplier(loadRatio);
        }

        private bool CanEquipToSlot(ItemInstance item, EquipmentSlot slot)
        {
            if (item == null) return false;

            if (item.Data is ArmorData armorData)
            {
                return armorData.slot == slot;
            }

            if (item.Data is AccessoryData)
            {
                return slot == EquipmentSlot.Accessory;
            }

            return false;
        }

        private ItemInstance UnequipInternal(EquipmentSlot slot)
        {
            if (!_slots.TryGetValue(slot, out ItemInstance item) || item == null)
            {
                return null;
            }

            EquipmentModifierService.RemoveItemModifiers(characterStats, item);
            _slots.Remove(slot);
            inventory.AddItem(item);
            return item;
        }
    }
}