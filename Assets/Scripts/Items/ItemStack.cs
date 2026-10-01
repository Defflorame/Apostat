namespace Game.Items
{
    /// <summary>
    /// Runtime-запись в инвентаре: конкретный экземпляр предмета + его
    /// количество (для стакающихся материалов/расходников; для
    /// нестакающихся предметов Quantity всегда 1).
    /// </summary>
    public class ItemStack
    {
        public ItemInstance Item { get; }
        public int Quantity { get; set; }

        public ItemStack(ItemInstance item, int quantity)
        {
            Item = item;
            Quantity = quantity;
        }
    }
}