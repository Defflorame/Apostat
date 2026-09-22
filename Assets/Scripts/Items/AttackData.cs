using UnityEngine;

namespace Game.Items
{
    public enum AttackType
    {
        Light,
        Heavy
    }

    /// <summary>
    /// Данные одной атаки. Range/VFX и т.д. будут добавлены вместе
    /// с системами, которым они нужны.
    /// </summary>
    [CreateAssetMenu(fileName = "NewAttackData", menuName = "Game/Combat/Attack Data")]
    public class AttackData : ScriptableObject
    {
        public AttackType attackType = AttackType.Light;
        public int damage = 10;

        [Header("Timing (seconds)")]
        public float startupDuration = 0.15f;
        public float activeDuration = 0.1f;
        public float recoveryDuration = 0.2f;

        [Header("Costs & Interactions")]
        public float staminaCost = 15f;
        public bool canBeBlocked = true;
        public bool canBeParried = true;
        [Tooltip("Зарезервировано для будущей системы AI: окно, в которое враг может быть парирован этой атакой.")]
        public float parryWindow = 0.3f;
        public float staggerPower = 10f;

        [Header("Charge (используется только Heavy Attack)")]
        [Tooltip("Множитель урона при полной зарядке. Для Light Attack оставить 1.")]
        public float maxChargeDamageMultiplier = 2f;
        [Tooltip("Сколько секунд удержания ЛКМ считается полным зарядом (100%). Для Light Attack не используется.")]
        public float maxChargeDuration = 1.2f;
    }
}
