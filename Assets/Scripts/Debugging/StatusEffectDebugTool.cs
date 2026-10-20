using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using Game.Combat;

namespace Game.Debugging
{
    /// <summary>
    /// ВРЕМЕННЫЙ инструмент. Не часть архитектуры статус-эффектов — удалите файл, когда
    /// появятся реальные источники эффектов (враги, предметы) и HUD (Phase 14).
    ///
    /// Numpad 1–9 накладывают на ТЕКУЩУЮ цель эффект с тем же номером из списка effects
    /// (в обход шанса и сопротивлений — проверяется сама механика эффекта: тики, статы, таймер).
    /// Numpad + / − переключают текущую цель между targets (игрок, манекен и т.д.).
    /// Чтобы проверить шанс и сопротивления, бейте оружием/заклинанием или клавишей T (TestAttacker).
    /// </summary>
    public class StatusEffectDebugTool : MonoBehaviour
    {
        [Tooltip("Цели по порядку: игрок, манекен и т.д. Нужен StatusEffectController на каждой.")]
        [SerializeField] private StatusEffectController[] targets;

        [Tooltip("Эффекты на Numpad 1–9 (по порядку). Максимум 9.")]
        [SerializeField] private StatusEffectData[] effects;

        private const int MaxEffects = 9;

        private int _targetIndex;
        private KeyControl[] _numpadKeys;

        private StatusEffectController CurrentTarget =>
            targets != null && targets.Length > 0 ? targets[Mathf.Clamp(_targetIndex, 0, targets.Length - 1)] : null;

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null) return;

            _numpadKeys ??= new[]
            {
                keyboard.numpad1Key, keyboard.numpad2Key, keyboard.numpad3Key,
                keyboard.numpad4Key, keyboard.numpad5Key, keyboard.numpad6Key,
                keyboard.numpad7Key, keyboard.numpad8Key, keyboard.numpad9Key
            };

            if (targets != null && targets.Length > 0)
            {
                if (keyboard.numpadPlusKey.wasPressedThisFrame)
                {
                    _targetIndex = (_targetIndex + 1) % targets.Length;
                    Debug.Log($"StatusEffectDebugTool: цель — {CurrentTarget?.name}");
                }

                if (keyboard.numpadMinusKey.wasPressedThisFrame)
                {
                    _targetIndex = (_targetIndex - 1 + targets.Length) % targets.Length;
                    Debug.Log($"StatusEffectDebugTool: цель — {CurrentTarget?.name}");
                }
            }

            int count = effects != null ? Mathf.Min(effects.Length, MaxEffects) : 0;
            for (int i = 0; i < count; i++)
            {
                if (_numpadKeys[i].wasPressedThisFrame)
                {
                    ApplyEffect(effects[i]);
                }
            }
        }

        private void ApplyEffect(StatusEffectData effect)
        {
            StatusEffectController target = CurrentTarget;

            if (target == null || effect == null)
            {
                Debug.LogWarning("StatusEffectDebugTool: не назначена цель или эффект для этой клавиши.", this);
                return;
            }

            Debug.Log($"StatusEffectDebugTool: накладываю {effect.name} на {target.name}");
            target.Apply(effect);
        }

        private void OnGUI()
        {
            StatusEffectController target = CurrentTarget;

            GUIStyle label = new GUIStyle(GUI.skin.label) { fontSize = 14 };
            label.normal.textColor = Color.white;
            GUIStyle header = new GUIStyle(label) { fontStyle = FontStyle.Bold };

            int effectCount = effects != null ? Mathf.Min(effects.Length, MaxEffects) : 0;
            int activeCount = target != null ? target.ActiveEffects.Count : 0;

            const int width = 320;
            const int padding = 10;
            int height = 60 + effectCount * 20 + 30 + activeCount * 20;

            // Под PlayerStateDebugOverlay (правый верхний угол).
            float x = Screen.width - width - padding;
            float y = 230f;

            GUI.Box(new Rect(x, y, width, height), "");
            GUILayout.BeginArea(new Rect(x + 10, y + 8, width - 20, height - 16));

            GUILayout.Label($"Status Debug — цель: {(target != null ? target.name : "-")}  (Numpad +/−)", header);
            GUILayout.Space(4);

            for (int i = 0; i < effectCount; i++)
            {
                string effectName = effects[i] != null ? effects[i].name : "-";
                GUILayout.Label($"Numpad {i + 1}: {effectName}", label);
            }

            GUILayout.Space(6);
            GUILayout.Label("Активные:", header);

            if (target != null)
            {
                foreach (StatusEffectInstance instance in target.ActiveEffects)
                {
                    GUILayout.Label($"{instance.Data.name}  {instance.RemainingTime:F1}s", label);
                }
            }

            GUILayout.EndArea();
        }
    }
}
