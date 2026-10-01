using UnityEngine;

namespace Game.Items
{
    /// <summary>
    /// Общая база данных предмета: то, чем предмет ЯВЛЯЕТСЯ (не хранит
    /// runtime state конкретного экземпляра — это делает ItemInstance).
    /// Конкретные типы (WeaponData, ShieldData, ArmorData, AccessoryData,
    /// RingData и т.д.) наследуются от неё.
    /// </summary>
    public abstract class ItemData : ScriptableObject
    {
        public string itemId;
        public string itemName;
        [TextArea] public string description;

        [Tooltip("Вес предмета — учитывается EquipmentModifierService при подсчёте нагрузки экипировки.")]
        public float weight = 0f;

        [Tooltip("Максимальный размер стака в инвентаре. 1 — предмет не стакуется (оружие, броня и т.д.).")]
        public int maxStackSize = 1;
    }
}