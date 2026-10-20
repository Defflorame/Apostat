using UnityEngine;
using Game.Items;
using Game.Player.Equipment;
using Game.Combat;

namespace Game.Player.ViewModel
{
    /// <summary>
    /// Координатор визуала рук: слушает смену экипировки и обновляет руки,
    /// оружие и щит. Ничего не решает в игровой логике, только читает
    /// PlayerEquipment/CombatLoadout.
    /// Правая рука видна всегда. Левая — только если есть щит или
    /// двуручное оружие. При dual-wield на экране только активное оружие.
    /// </summary>
    public class ViewModelController : MonoBehaviour
    {
        [Header("Источники данных")]
        [SerializeField] private PlayerEquipment playerEquipment;
        [SerializeField] private CombatLoadout combatLoadout;

        [Header("Правая рука")]
        [SerializeField] private HandVisual rightHand;
        [SerializeField] private HeldItemSocket rightSocket;

        [Header("Левая рука")]
        [SerializeField] private HandVisual leftHand;
        [SerializeField] private HeldItemSocket leftSocket;

        private void OnEnable()
        {
            playerEquipment.OnEquipmentChanged += Refresh;
            combatLoadout.OnLoadoutChanged += Refresh;
        }

        private void OnDisable()
        {
            playerEquipment.OnEquipmentChanged -= Refresh;
            combatLoadout.OnLoadoutChanged -= Refresh;
        }

        private void Start()
        {
            Refresh();
        }

        private void Refresh()
        {
            HandEquipment hands = playerEquipment.Hands;
            WeaponData weapon = hands.ActiveWeapon;
            ShieldData shield = hands.CurrentShield;
            bool twoHanded = weapon != null && weapon.isTwoHanded;

            ItemInstance glovesItem = playerEquipment.GetEquipped(EquipmentSlot.Gloves);
            GameObject handPrefab = (glovesItem?.Data as ArmorData)?.handPrefab;

            // Модель левой руки может висеть на хвате старого оружия — вернуть ДО его замены.
            leftHand.SetModelAnchor(null);

            rightHand.SetGlove(handPrefab);
            leftHand.SetGlove(handPrefab);

            rightSocket.Attach(weapon != null ? weapon.prefab : null);
            leftSocket.Attach(shield != null ? shield.prefab : null);

            leftHand.SetVisible(shield != null || twoHanded);

            if (twoHanded)
            {
                ItemGrip grip = rightSocket.SecondaryGrip;
                if (grip != null)
                {
                    leftHand.SetModelAnchor(grip.transform);
                }
                else
                {
                    Debug.LogWarning($"ViewModelController: на префабе '{weapon.itemName}' нет ItemGrip типа Secondary — левая рука остаётся на месте.", this);
                }
            }
        }
    }
}