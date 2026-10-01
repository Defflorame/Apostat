using System;
using UnityEngine;
using Game.Items;

namespace Game.Player.Equipment
{
    /// <summary>
    /// Read-only срез HandEquipment для Combat-систем. CurrentWeapon отдаёт
    /// именно АКТИВНОЕ оружие (при dual-wield — то, что выбрано игроком),
    /// поэтому WeaponAttackController по-прежнему ничего не знает про
    /// количество оружия в руках.
    /// </summary>
    public class CombatLoadout : MonoBehaviour
    {
        [SerializeField] private HandEquipment handEquipment;

        public WeaponData CurrentWeapon => handEquipment.ActiveWeapon;
        public ShieldData CurrentShield => handEquipment.CurrentShield;
        public bool HasShield => CurrentShield != null;
        public bool IsDualWielding => handEquipment.IsDualWielding;

        public event Action OnLoadoutChanged;

        private void OnEnable()
        {
            handEquipment.OnHandEquipmentChanged += HandleHandEquipmentChanged;
        }

        private void OnDisable()
        {
            handEquipment.OnHandEquipmentChanged -= HandleHandEquipmentChanged;
        }

        /// <summary>Переключить активную руку при двух одноручных оружиях.</summary>
        public bool TrySwitchActiveWeapon()
        {
            return handEquipment.SwitchActiveWeapon();
        }

        private void HandleHandEquipmentChanged()
        {
            OnLoadoutChanged?.Invoke();
        }
    }
}