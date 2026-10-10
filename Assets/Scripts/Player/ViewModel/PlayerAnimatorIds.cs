using UnityEngine;

namespace Game.Player.ViewModel
{
    /// <summary>
    /// Единственное место, где живут имена параметров Animator и имена клипов-слотов.
    /// Строковые имена нужны Editor-скрипту, который создаёт контроллеры.
    /// </summary>
    public static class PlayerAnimatorIds
    {
        // Параметры правой руки (имена)
        public const string IsLightAttackingName = "IsLightAttacking";
        public const string IsHeavyAttackingName = "IsHeavyAttacking";
        public const string IsChargingName = "IsCharging";
        public const string IsDodgingName = "IsDodging";
        public const string IsParryingName = "IsParrying";
        public const string AttackSpeedName = "AttackSpeed";
        public const string LightVariantName = "LightVariant";

        // Параметры левой руки (имена)
        public const string IsBlockingName = "IsBlocking";

        // Хэши для кода
        public static readonly int IsLightAttacking = Animator.StringToHash(IsLightAttackingName);
        public static readonly int IsHeavyAttacking = Animator.StringToHash(IsHeavyAttackingName);
        public static readonly int IsCharging = Animator.StringToHash(IsChargingName);
        public static readonly int IsDodging = Animator.StringToHash(IsDodgingName);
        public static readonly int IsParrying = Animator.StringToHash(IsParryingName);
        public static readonly int AttackSpeed = Animator.StringToHash(AttackSpeedName);
        public static readonly int LightVariant = Animator.StringToHash(LightVariantName);
        public static readonly int IsBlocking = Animator.StringToHash(IsBlockingName);

        // Имена клипов-слотов (по ним работают оверрайды)
        public const string IdleClip = "Idle";
        public const string LightAttackClip = "LightAttack";
        public const string LightAttack2Clip = "LightAttack2";
        public const string LightAttack3Clip = "LightAttack3";
        public static readonly string[] LightAttackClips = { LightAttackClip, LightAttack2Clip, LightAttack3Clip };
        public const string ChargeHoldClip = "ChargeHold";
        public const string HeavyAttackClip = "HeavyAttack";
        public const string DodgeClip = "Dodge";
        public const string ParryClip = "Parry";
        public const string BlockRaiseClip = "BlockRaise";
    }
}