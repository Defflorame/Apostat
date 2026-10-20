using UnityEngine;
using Game.Combat;

namespace Game.Items
{
    /// <summary>
    /// Описывает, чем оружие является. itemName/description/weight
    /// унаследованы от ItemData — общей базы для инвентаря и экипировки
    /// (Phase 4). Не хранит runtime state.
    /// </summary>
    [CreateAssetMenu(fileName = "NewWeaponData", menuName = "Game/Items/Weapon Data")]
    public class WeaponData : ItemData
    {
        public AttackData lightAttack;

        [Tooltip("Вторая и третья лёгкие атаки серии (необязательно). Максимум 2.")]
        public AttackData[] lightAttackFollowUps;

        public AttackData heavyAttack;

        [Header("Elemental damage")]
        [Tooltip("Стихия оружия. Пусто — стихийной части нет.")]
        public ElementType element;
        [Min(0)]
        [Tooltip("Стихийный урон оружия — идёт ПОВЕРХ физического урона атаки. Каждая атака масштабирует его через свой elementalMultiplier.")]
        public int elementalDamage = 0;

        [Header("Status effects on hit")]
        [Tooltip("Эффекты, которые оружие пытается наложить на цель, если по ней прошёл урон (яд на клинке и т.п.). Действуют для всех атак этого оружия.")]
        public StatusEffectApplication[] onHitEffects;

        [Header("Weapon-level stats")]
        [Tooltip("Длина хитбокса атаки в метрах вдоль forward оружия.")]
        public float range = 1f;
        [Tooltip("Дистанция отскока (ПКМ без щита).")]
        public float dodgeDistance = 3f;

        [Header("Hands")]
        [Tooltip("Двуручное оружие занимает оба слота (MainHand + OffHand) и блокирует щит/второе оружие.")]
        public bool isTwoHanded = false;

        [Tooltip("Модель оружия для вида от первого лица. БЕЗ коллайдеров и WeaponHitbox. Дочерние ItemGrip: Primary — где держит правая рука, Secondary (двуручное) — где держит левая.")]
        public GameObject prefab;

        [Tooltip("Клипы этого оружия для правой руки: Override от RightHand_Base. Пусто — используется базовый контроллер (кулаки).")]
        public AnimatorOverrideController animatorOverride;

                public const int MaxLightAttacks = 3;

        /// <summary>Стихийный урон оружия для конкретной атаки (с её elementalMultiplier). 0 — если стихии нет.</summary>
        public int GetElementalDamage(AttackData attack)
        {
            if (element == null || elementalDamage <= 0 || attack == null) return 0;
            return Mathf.RoundToInt(elementalDamage * attack.elementalMultiplier);
        }

        /// <summary>Сколько лёгких атак в серии этого оружия (0, если lightAttack не назначен).</summary>
        public int LightAttackCount
        {
            get
            {
                if (lightAttack == null) return 0;

                int count = 1;
                if (lightAttackFollowUps != null)
                {
                    for (int i = 0; i < lightAttackFollowUps.Length && count < MaxLightAttacks; i++)
                    {
                        if (lightAttackFollowUps[i] != null) count++;
                    }
                }

                return count;
            }
        }

        /// <summary>index 0 — первая атака, дальше по порядку из lightAttackFollowUps.</summary>
        public AttackData GetLightAttack(int index)
        {
            if (index <= 0 || lightAttackFollowUps == null) return lightAttack;

            int found = 0;
            foreach (AttackData followUp in lightAttackFollowUps)
            {
                if (followUp == null) continue;

                found++;
                if (found == index) return followUp;
            }

            return lightAttack;
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            int maxFollowUps = MaxLightAttacks - 1;
            if (lightAttackFollowUps != null && lightAttackFollowUps.Length > maxFollowUps)
            {
                System.Array.Resize(ref lightAttackFollowUps, maxFollowUps);
            }
        }
    }
}