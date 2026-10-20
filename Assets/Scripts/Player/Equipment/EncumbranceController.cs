using UnityEngine;
using Game.Combat;
using Game.Stats;
using Game.Player.Stats;

namespace Game.Player.Equipment
{
    /// <summary>
    /// Переносит штраф от веса экипировки на стоимость стамины.
    /// Формулы — в EncumbranceService, здесь только связка.
    /// </summary>
    public class EncumbranceController : MonoBehaviour
    {
        [SerializeField] private PlayerEquipment playerEquipment;
        [SerializeField] private CharacterStats characterStats;
        [SerializeField] private StaminaComponent stamina;

        private void OnEnable()
        {
            playerEquipment.OnEquipmentChanged += Refresh;
            characterStats.OnStatsChanged += HandleStatsChanged;
        }

        private void OnDisable()
        {
            playerEquipment.OnEquipmentChanged -= Refresh;
            characterStats.OnStatsChanged -= HandleStatsChanged;
        }

        private void Start() => Refresh();

        private void HandleStatsChanged(DerivedCharacterStats _) => Refresh();

        private void Refresh()
        {
            stamina.SetCostMultiplier(EncumbranceService.GetStaminaCostMultiplier(playerEquipment.GetLoadRatio()));
        }
    }
}