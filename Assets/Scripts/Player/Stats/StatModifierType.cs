namespace Game.Player.Stats
{
    /// <summary>
    /// Способ применения StatModifier к базовому значению характеристики.
    /// </summary>
    public enum StatModifierType
    {
        /// <summary>Прибавляется к базовому значению до применения процентов.</summary>
        Flat,

        /// <summary>Проценты от суммы (базовое значение + Flat-модификаторы).</summary>
        Percent
    }
}
