namespace Game.Items
{
    /// <summary>
    /// Runtime-экземпляр предмета: ссылка на статичные данные (ItemData).
    /// WeaponData описывает Iron Sword как тип; конкретное состояние
    /// экземпляра (в будущем — уровень заточки, Phase 12 WeaponUpgrade)
    /// будет храниться здесь, а не в ScriptableObject. Два ItemInstance
    /// с одинаковым Data — разные предметы (сравниваются по ссылке).
    /// </summary>
    public class ItemInstance
    {
        public ItemData Data { get; }

        public ItemInstance(ItemData data)
        {
            Data = data;
        }
    }
}