using UnityEngine;
using UnityEngine.InputSystem;
using Game.Combat;
using Game.Player.Stats;

namespace Game.Debugging
{
    /// <summary>
    /// ВРЕМЕННЫЙ инструмент. Не часть архитектуры прогрессии — удалите
    /// этот файл, когда появится реальная система прокачки
    /// (ProgressionSystem/AbilityPointController, Phase 7).
    ///
    /// Нужен, чтобы вручную проверить прямо сейчас, что StatModifier
    /// действительно меняет DerivedCharacterStats и синхронизируется с
    /// HealthComponent/StaminaComponent/ManaComponent, не дожидаясь UI
    /// и системы прокачки.
    ///
    /// Клавиши 1-7 добавляют +1 очко к соответствующей характеристике,
    /// R — снимает все модификаторы, добавленные этим инструментом.
    /// </summary>
    public class CharacterStatsDebugTool : MonoBehaviour
    {
        [SerializeField] private CharacterStats characterStats;
        [SerializeField] private HealthComponent healthComponent;
        [SerializeField] private StaminaComponent staminaComponent;
        [SerializeField] private ManaComponent manaComponent;

        [Tooltip("На сколько увеличивать характеристику за одно нажатие.")]
        [SerializeField] private float pointsPerPress = 1f;

        private void Update()
        {
            if (Keyboard.current == null || characterStats == null) return;

            if (Keyboard.current.digit1Key.wasPressedThisFrame) AddPoint(StatType.Strength);
            if (Keyboard.current.digit2Key.wasPressedThisFrame) AddPoint(StatType.Stamina);
            if (Keyboard.current.digit3Key.wasPressedThisFrame) AddPoint(StatType.Mana);
            if (Keyboard.current.digit4Key.wasPressedThisFrame) AddPoint(StatType.MagicDamage);
            if (Keyboard.current.digit5Key.wasPressedThisFrame) AddPoint(StatType.Speed);
            if (Keyboard.current.digit6Key.wasPressedThisFrame) AddPoint(StatType.CarryWeight);
            if (Keyboard.current.digit7Key.wasPressedThisFrame) AddPoint(StatType.Defense);

            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                characterStats.RemoveModifiersFromSource(this);
                Debug.Log("CharacterStatsDebugTool: все тестовые модификаторы сняты.");
            }
        }

        private void AddPoint(StatType stat)
        {
            characterStats.AddModifier(new StatModifier(stat, StatModifierType.Flat, pointsPerPress, this));
        }

        private void OnGUI()
        {
            if (characterStats == null || characterStats.Stats == null) return;

            DerivedCharacterStats stats = characterStats.Stats;

            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.fontSize = 22;
            labelStyle.normal.textColor = Color.white;
            GUIStyle headerStyle = new GUIStyle(labelStyle);
            headerStyle.fontSize = 14;
            GUI.Box(new Rect(10, 10, 450, 360), "");
            GUILayout.BeginArea(new Rect(25, 20, 420, 340));
            GUILayout.Label("Stat Debug: 1-7 = +1 очко, R = сброс", headerStyle);
            GUILayout.Label("1 Str  2 Sta  3 Mana  4 MagDmg  5 Spd  6 Carry  7 Def", headerStyle);
            GUILayout.Space(10);

            if (healthComponent != null)
                GUILayout.Label($"HP: {healthComponent.CurrentHealth} / {stats.MaxHealth}", labelStyle);
            if (staminaComponent != null)
                GUILayout.Label($"Stamina: {staminaComponent.CurrentStamina:F0} / {stats.MaxStamina:F0} (regen {stats.StaminaRegenPerSecond:F1}/s)", labelStyle);
            if (manaComponent != null)
                GUILayout.Label($"Mana: {manaComponent.CurrentMana:F0} / {stats.MaxMana:F0}", labelStyle);

            GUILayout.Space(5);

            GUILayout.Label($"Physical Damage Bonus: {stats.PhysicalDamageBonusPercent:F1}%", labelStyle);
            GUILayout.Label($"Magic Damage: {stats.MagicDamage:F1}", labelStyle);
            GUILayout.Label($"Defense: {stats.DefenseValue:F1}", labelStyle);
            GUILayout.Label($"Move Speed: {stats.MovementSpeed:F2}", labelStyle);
            GUILayout.Label($"Carry Weight Limit: {stats.CarryWeightLimit:F1}", labelStyle);

            GUILayout.EndArea();
        }

    }
}