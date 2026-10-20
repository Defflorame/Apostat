using UnityEngine;
using Game.Items;

namespace Game.Combat
{
    /// <summary>
    /// Описывает временный эффект: сколько длится, какие модификаторы характеристик
    /// даёт и наносит ли периодический урон. Не хранит runtime state (оставшееся
    /// время — в StatusEffectInstance). Яд, кровотечение, ожог, замедление, проклятие —
    /// это разные ассеты одного класса: комбинацией полей, без отдельного кода.
    /// </summary>
    [CreateAssetMenu(fileName = "NewStatusEffect", menuName = "Game/Combat/Status Effect Data")]
    public class StatusEffectData : ScriptableObject
    {
        public string effectName;

        [Tooltip("Сколько секунд действует эффект.")]
        public float duration = 10f;

        [Header("Periodic damage (яд, кровотечение, ожог)")]
        [Min(0f)]
        [Tooltip("Раз в сколько секунд наносится урон. Используется, только если урон тика > 0.")]
        public float tickInterval = 1f;

        [Min(0)]
        [Tooltip("Плоский урон за один тик.")]
        public int tickDamage = 0;

        [Range(0f, 100f)]
        [Tooltip("Урон за тик в процентах от МАКСИМАЛЬНОГО HP цели. Складывается с плоским уроном. Для кровотечения: например, 2 = 2% макс. HP за тик.")]
        public float tickMaxHealthPercent = 0f;

        [Tooltip("Если включено, защита (Defense) цели не уменьшает урон тика.")]
        public bool tickIgnoresDefense = false;

        [Tooltip("Стихия урона тика (например, Fire для ожога). Пусто — физический урон.")]
        public ElementType tickElement;
        [Header("Stat modifiers")]
        [Tooltip("Модификаторы характеристик на время действия (замедление, проклятие, баффы). Для PhysicalDamageBonus используйте тип Flat (значение = проценты урона).")]
        public StatModifierDefinition[] statModifiers;

        public bool HasPeriodicDamage => (tickDamage > 0 || tickMaxHealthPercent > 0f) && tickInterval > 0f;

    }
}
