using UnityEngine;
using Game.Player.Stats;
using Game.Player.Equipment;

namespace Game.Debugging
{
    /// <summary>
    /// ВРЕМЕННЫЙ инструмент. Не часть архитектуры — удалить, когда появится
    /// реальный HUD (Phase 14). Только читает PlayerEquipment/CharacterStats/
    /// EncumbranceService — ничего не меняет.
    ///
    /// Показывает в левом верхнем углу текущий вес экипировки, лимит,
    /// loadRatio и итоговые множители скорости/стамины, применённые
    /// сейчас к игроку.
    /// </summary>
    public class EncumbranceDebugOverlay : MonoBehaviour
    {
        [SerializeField] private PlayerEquipment playerEquipment;
        [SerializeField] private CharacterStats characterStats;

        private void OnGUI()
        {
            if (playerEquipment == null || characterStats == null) return;

            float currentWeight = playerEquipment.GetTotalWeight();
            float weightLimit = characterStats.Stats.CarryWeightLimit;
            float loadRatio = EncumbranceService.GetLoadRatio(currentWeight, weightLimit);
            float movementMultiplier = EncumbranceService.GetMovementMultiplier(loadRatio);
            float staminaCostMultiplier = EncumbranceService.GetStaminaCostMultiplier(loadRatio);

            GUIStyle label = new GUIStyle(GUI.skin.label) { fontSize = 16 };
            label.normal.textColor = Color.white;
            GUIStyle header = new GUIStyle(label) { fontSize = 14, fontStyle = FontStyle.Bold };

            const int width = 280;
            const int height = 150;
            const int padding = 10;

            GUI.Box(new Rect(padding, Screen.height - height - padding, width, height), "");
            GUILayout.BeginArea(new Rect(padding + 10, Screen.height - height - padding + 8, width - 20, height - 16));

            GUILayout.Label("Encumbrance", header);
            GUILayout.Label($"Weight: {currentWeight:F1} / {weightLimit:F1}", label);
            GUILayout.Label($"Load ratio: {loadRatio:F2}", label);
            GUILayout.Space(6);
            GUILayout.Label($"Movement x{movementMultiplier:F2}", label);
            GUILayout.Label($"Stamina cost x{staminaCostMultiplier:F2}", label);

            GUILayout.EndArea();
        }
    }
}