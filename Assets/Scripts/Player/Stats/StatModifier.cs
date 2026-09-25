namespace Game.Player.Stats
{
    /// <summary>
    /// Runtime-описание одного модификатора характеристики (например, бонус
    /// от предмета экипировки или временного эффекта). Это чистые данные —
    /// сам объект ничего не пересчитывает, этим занимается CharacterStats.
    ///
    /// Source нужен, чтобы впоследствии снять именно свои модификаторы,
    /// не затронув чужие (см. CharacterStats.RemoveModifiersFromSource) —
    /// например, при снятии конкретного кольца или предмета экипировки.
    /// Ничего в проекте пока не создаёт такие модификаторы (Equipment —
    /// Phase 4, StatusEffects — Phase 6), но именно ради них существует
    /// сам класс StatModifier, поэтому Source закладывается сразу.
    /// </summary>
    public class StatModifier
    {
        public StatType Stat { get; }
        public StatModifierType Type { get; }
        public float Value { get; }
        public object Source { get; }

        public StatModifier(StatType stat, StatModifierType type, float value, object source)
        {
            Stat = stat;
            Type = type;
            Value = value;
            Source = source;
        }
    }
}
