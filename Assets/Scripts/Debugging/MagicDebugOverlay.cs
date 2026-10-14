using UnityEngine;
using UnityEngine.InputSystem;
using Game.Combat;
using Game.Magic;

namespace Game.Debugging
{
    /// <summary>
    /// ВРЕМЕННЫЙ инструмент. Удалить, когда появится HUD (Phase 14).
    /// Показывает активное кольцо/заклинание, кулдаун, ману и HP.
    /// Клавиша M — восполнить ману (предметов маны пока нет).
    /// </summary>
    public class MagicDebugOverlay : MonoBehaviour
    {
        [SerializeField] private RingSpellController ringSpellController;
        [SerializeField] private SpellCaster spellCaster;
        [SerializeField] private ManaComponent mana;
        [SerializeField] private HealthComponent health;

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.mKey.wasPressedThisFrame)
            {
                mana.Restore(9999f);
            }
        }

        private void OnGUI()
        {
            GUIStyle label = new GUIStyle(GUI.skin.label) { fontSize = 16 };
            label.normal.textColor = Color.white;

            const int width = 340;
            const int height = 120;
            float x = Screen.width * 0.5f - width * 0.5f;
            float y = Screen.height - height - 10;

            GUI.Box(new Rect(x, y, width, height), "");
            GUILayout.BeginArea(new Rect(x + 10, y + 8, width - 20, height - 16));

            SpellData spell = ringSpellController.ActiveSpell;
            GUILayout.Label($"Ring: {ringSpellController.ActiveRing?.Data.itemName ?? "-"}", label);
            GUILayout.Label($"Spell: {spell?.spellName ?? "-"}  ({spell?.resource} {spell?.cost})", label);
            GUILayout.Label($"Cooldown: {spellCaster.GetCooldownRemaining(spell):F2}s", label);
            GUILayout.Label($"Mana: {mana.CurrentMana:F0}/{mana.MaxMana:F0}   HP: {health.CurrentHealth}   (M — мана)", label);

            GUILayout.EndArea();
        }
    }
}