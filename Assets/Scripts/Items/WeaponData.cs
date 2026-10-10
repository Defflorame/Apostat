using UnityEngine;

namespace Game.Items
{
    /// <summary>
    /// Описывает, чем оружие является. itemId/itemName/description/weight
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

        private void OnValidate()
        {
            int maxFollowUps = MaxLightAttacks - 1;
            if (lightAttackFollowUps != null && lightAttackFollowUps.Length > maxFollowUps)
            {
                System.Array.Resize(ref lightAttackFollowUps, maxFollowUps);
            }
        }
    }
}