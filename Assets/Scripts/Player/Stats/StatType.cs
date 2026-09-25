namespace Game.Player.Stats
{
    /// <summary>
    /// Базовые характеристики персонажа, к которым может применяться
    /// StatModifier. Производные величины (регенерация стамины, итоговый
    /// объём маны, бонус урона от силы, значение защиты) сюда не входят —
    /// они всегда пересчитываются CharacterStats из этих базовых значений.
    /// </summary>
    public enum StatType
    {
        Strength,
        Stamina,
        Mana,
        MagicDamage,
        Speed,
        CarryWeight,
        Defense
    }
}