using System.Collections.Generic;

namespace Game.Items
{
    /// <summary>
    /// Реализуют предметы, дающие бонусы к характеристикам при экипировке
    /// (броня, аксессуары, кольца). Единая точка для EquipmentModifierService —
    /// не нужно дублировать применение модификаторов для каждого типа предмета.
    /// </summary>
    public interface IStatModifierProvider
    {
        IReadOnlyList<StatModifierDefinition> StatModifiers { get; }
    }
}