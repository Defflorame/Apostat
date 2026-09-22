using UnityEngine;

namespace Game.Items
{
    /// <summary>
    /// Описывает, чем оружие является (не хранит runtime state, например
    /// уровень заточки — тот появится в ItemInstance на этапе инвентаря).
    /// Поля Weight, WeaponType, Requirements и т.д. будут добавлены,
    /// когда появятся системы, которые их используют (Phase 3-4).
    /// </summary>
    [CreateAssetMenu(fileName = "NewWeaponData", menuName = "Game/Items/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        public string weaponId;
        public string weaponName;
        [TextArea] public string description;

        public AttackData lightAttack;
        public AttackData heavyAttack;

        [Header("Weapon-level stats")]
        [Tooltip("Длина хитбокса атаки в метрах вдоль forward оружия.")]
        public float range = 1f;
        [Tooltip("Дистанция отскока (ПКМ без щита). Пока не используется в Phase 1 — понадобится, когда импортируете Phase 2 (DodgeState).")]
        public float dodgeDistance = 3f;
    }
}
