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
        public AttackData heavyAttack;

        [Header("Weapon-level stats")]
        [Tooltip("Длина хитбокса атаки в метрах вдоль forward оружия.")]
        public float range = 1f;
        [Tooltip("Дистанция отскока (ПКМ без щита).")]
        public float dodgeDistance = 3f;

        [Header("Hands")]
        [Tooltip("Двуручное оружие занимает оба слота (MainHand + OffHand) и блокирует щит/второе оружие.")]
        public bool isTwoHanded = false;
    }
}