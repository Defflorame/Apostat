using UnityEngine;

namespace Game.Player.Stats
{
    /// <summary>
    /// Описывает базовые характеристики персонажа. Health и Mana больше
    /// не самостоятельные инвестируемые характеристики: Health — производная
    /// от Strength (сила даёт и бонус физ. урона, и живучесть), Mana теперь
    /// инвестируется напрямую (без промежуточного Intelligence, по аналогии
    /// с тем, как уже устроена Stamina). Defense — новая боевая характеристика,
    /// снижающая входящий урон; сама по себе ничего не делает, пока в Phase 4
    /// не появится броня и DamageResolver.ApplyDefense(). Не хранит runtime
    /// состояние конкретного прохождения — этим занимаются
    /// CharacterStats/DerivedCharacterStats.
    /// </summary>
    [CreateAssetMenu(fileName = "NewBaseCharacterStats", menuName = "Game/Player/Base Character Stats")]
    public class BaseCharacterStats : ScriptableObject
    {
        [Header("Strength")]
        public float strength = 10f;

        [Tooltip("Здоровье персонажа без учёта Strength (базовая живучесть на 0 очков силы).")]
        public int baseHealth = 50;

        [Tooltip("Сколько единиц MaxHealth даёт одно очко Strength.")]
        public float healthPerStrengthPoint = 5f;

        [Tooltip("Сколько процентов бонуса к физическому урону даёт одно очко Strength.")]
        public float physicalDamageBonusPerStrengthPoint = 1f;

        [Header("Stamina")]
        [Tooltip("Определяет и максимальный запас выносливости, и скорость её восстановления.")]
        public float stamina = 100f;
        public float staminaRegenPerSecond = 15f;

        [Header("Mana")]
        [Tooltip("Запас маны.")]
        public float mana = 50f;

        [Header("Combat")]
        public float magicDamage = 5f;

        [Header("Defense")]
        public float defense = 0f;
        [Tooltip("Сколько единиц входящего урона снимает одно очко Defense.")]
        public float damageReductionPerDefensePoint = 1f;

        [Header("Movement")]
        public float speed = 5f;

        [Header("Encumbrance")]
        [Tooltip("Максимальный вес экипировки, который персонаж может нести без штрафов.")]
        public float carryWeight = 40f;
    }
}