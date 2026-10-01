namespace Game.Items
{
    /// <summary>
    /// Слоты фиксированной экипировки. Руки (оружие/щит) сюда не входят —
    /// их описывает HandEquipment. Кольца тоже не входят — их количество
    /// не должно быть захардкожено (раздел 37 документа), поэтому кольца
    /// хранятся отдельным списком в PlayerEquipment.
    /// </summary>
    public enum EquipmentSlot
    {
        Head,
        Body,
        Gloves,
        Legs,
        Neck,
        Accessory
    }
}