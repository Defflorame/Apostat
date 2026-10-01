using Game.Player.Stats;

namespace Game.Items
{
    /// <summary>
    /// Данные одного модификатора характеристики, задаваемого предметом
    /// в инспекторе. Runtime-применением (создание StatModifier с нужным
    /// Source и его снятием) занимается EquipmentModifierService.
    /// </summary>
    [System.Serializable]
    public struct StatModifierDefinition
    {
        public StatType stat;
        public StatModifierType type;
        public float value;
    }
}