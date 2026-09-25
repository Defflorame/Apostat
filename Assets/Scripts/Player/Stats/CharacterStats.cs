using System;
using System.Collections.Generic;
using UnityEngine;
using Game.Combat;

namespace Game.Player.Stats
{
    /// <summary>
    /// Хранит базовые характеристики персонажа (BaseCharacterStats) и
    /// активные StatModifier, пересчитывает DerivedCharacterStats и
    /// синхронизирует Max-значения с HealthComponent/StaminaComponent/
    /// ManaComponent. НЕ содержит боевых формул, не читает ввод и не
    /// решает, когда персонаж должен получить урон или потратить ресурс —
    /// это остаётся зоной ответственности Combat-систем. PhysicalDamageBonusPercent
    /// и DefenseValue тоже не применяются здесь — CharacterStats лишь
    /// вычисляет их и хранит в Stats, использовать их будет DamageResolver.
    /// </summary>
    public class CharacterStats : MonoBehaviour
    {
        [SerializeField] private BaseCharacterStats baseStats;

        [Header("Синхронизация Max-значений (необязательно)")]
        [Tooltip("Если назначено — CharacterStats выставляет их Max/Regen при пересчёте.")]
        [SerializeField] private HealthComponent healthComponent;
        [SerializeField] private StaminaComponent staminaComponent;
        [SerializeField] private ManaComponent manaComponent;

        public DerivedCharacterStats Stats { get; private set; }

        public event Action<DerivedCharacterStats> OnStatsChanged;

        private readonly List<StatModifier> _modifiers = new List<StatModifier>();

        private void Awake()
        {
            Recalculate();
            ApplyToComponents(refillToFull: true);
        }

        public void AddModifier(StatModifier modifier)
        {
            _modifiers.Add(modifier);
            Recalculate();
            ApplyToComponents(refillToFull: false);
            OnStatsChanged?.Invoke(Stats);
        }

        /// <summary>Снимает все модификаторы, добавленные конкретным источником (например, снятым кольцом).</summary>
        public void RemoveModifiersFromSource(object source)
        {
            int removed = _modifiers.RemoveAll(m => m.Source == source);
            if (removed == 0) return;

            Recalculate();
            ApplyToComponents(refillToFull: false);
            OnStatsChanged?.Invoke(Stats);
        }

        private void Recalculate()
        {
            if (baseStats == null)
            {
                Debug.LogWarning("CharacterStats: BaseCharacterStats не назначен.", this);
                Stats = new DerivedCharacterStats(1, 1f, 0f, 0f, 0f, 0f, 0f, 0f, 0f);
                return;
            }

            float strength = ApplyModifiers(StatType.Strength, baseStats.strength);
            int maxHealth = baseStats.baseHealth + Mathf.RoundToInt(strength * baseStats.healthPerStrengthPoint);
            float physicalDamageBonusPercent = strength * baseStats.physicalDamageBonusPerStrengthPoint;

            float maxStamina = ApplyModifiers(StatType.Stamina, baseStats.stamina);
            float maxMana = ApplyModifiers(StatType.Mana, baseStats.mana);
            float magicDamage = ApplyModifiers(StatType.MagicDamage, baseStats.magicDamage);
            float speed = ApplyModifiers(StatType.Speed, baseStats.speed);
            float carryWeight = ApplyModifiers(StatType.CarryWeight, baseStats.carryWeight);

            float defenseStat = ApplyModifiers(StatType.Defense, baseStats.defense);
            float defenseValue = defenseStat * baseStats.damageReductionPerDefensePoint;

            Stats = new DerivedCharacterStats(
                maxHealth: Mathf.Max(1, maxHealth),
                maxStamina: Mathf.Max(0f, maxStamina),
                staminaRegenPerSecond: baseStats.staminaRegenPerSecond,
                physicalDamageBonusPercent: Mathf.Max(0f, physicalDamageBonusPercent),
                maxMana: Mathf.Max(0f, maxMana),
                magicDamage: magicDamage,
                defenseValue: Mathf.Max(0f, defenseValue),
                movementSpeed: Mathf.Max(0f, speed),
                carryWeightLimit: Mathf.Max(0f, carryWeight));

            Debug.Log($"CharacterStats: recalculated -> HP {Stats.MaxHealth}, Stamina {Stats.MaxStamina:F1} (regen {Stats.StaminaRegenPerSecond:F1}/s), " +
                      $"Mana {Stats.MaxMana:F1}, PhysicalDamageBonus {Stats.PhysicalDamageBonusPercent:F1}%, MagicDamage {Stats.MagicDamage:F1}, " +
                      $"Defense {Stats.DefenseValue:F1}, Speed {Stats.MovementSpeed:F1}, CarryWeight {Stats.CarryWeightLimit:F1}");
        }

        /// <summary>(base + сумма Flat) * (1 + сумма Percent / 100). Общая формула для всех характеристик.</summary>
        private float ApplyModifiers(StatType stat, float baseValue)
        {
            float flatSum = 0f;
            float percentSum = 0f;

            for (int i = 0; i < _modifiers.Count; i++)
            {
                StatModifier modifier = _modifiers[i];
                if (modifier.Stat != stat) continue;

                if (modifier.Type == StatModifierType.Flat)
                {
                    flatSum += modifier.Value;
                }
                else
                {
                    percentSum += modifier.Value;
                }
            }

            return (baseValue + flatSum) * (1f + percentSum / 100f);
        }

        private void ApplyToComponents(bool refillToFull)
        {
            healthComponent?.SetMaxHealth(Stats.MaxHealth, refillToFull);
            staminaComponent?.SetMaxStamina(Stats.MaxStamina, refillToFull);
            staminaComponent?.SetRegenPerSecond(Stats.StaminaRegenPerSecond);
            manaComponent?.SetMaxMana(Stats.MaxMana, refillToFull);
        }
    }
}