using UnityEngine;
using UnityEngine.InputSystem;
using Game.Player;

namespace Game.Debugging
{
    /// <summary>
    /// ВРЕМЕННЫЙ инструмент. Не часть архитектуры — удалить вместе с
    /// остальными дебаг-окнами, когда они станут не нужны.
    ///
    /// Ctrl — включает/выключает "режим дебага": курсор становится видимым
    /// и разблокированным (можно кликать по OnGUI-окнам), поворот камеры
    /// блокируется через PlayerLook.SetLookEnabled(false).
    /// </summary>
    public class DebugCursorToggle : MonoBehaviour
    {
        [SerializeField] private PlayerLook playerLook;

        [Tooltip("Необязательно. Если назначен — в режиме дебага переключается на UI Action Map, " +
                 "и клики по дебаг-окнам не вызывают атаки/блок у игрока.")]
        [SerializeField] private InputManager inputManager;

        [Tooltip("false — Ctrl переключает режим по нажатию; true — режим активен, пока Ctrl удерживается.")]
        [SerializeField] private bool holdInsteadOfToggle = false;

        private bool _debugMode;

        private void Start()
        {
            ApplyDebugMode(false);
        }

        private void OnDisable()
        {
            if (_debugMode)
            {
                ApplyDebugMode(false);
            }
        }

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null) return;

            if (holdInsteadOfToggle)
            {
                bool held = keyboard.leftCtrlKey.isPressed || keyboard.rightCtrlKey.isPressed;
                if (held != _debugMode)
                {
                    ApplyDebugMode(held);
                }
            }
            else if (keyboard.leftCtrlKey.wasPressedThisFrame || keyboard.rightCtrlKey.wasPressedThisFrame)
            {
                ApplyDebugMode(!_debugMode);
            }
        }

        private void ApplyDebugMode(bool enabled)
        {
            _debugMode = enabled;

            Cursor.lockState = enabled ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = enabled;

            if (playerLook != null)
            {
                playerLook.SetLookEnabled(!enabled);
            }

            if (inputManager != null)
            {
                if (enabled) inputManager.SwitchToUI();
                else inputManager.SwitchToPlayer();
            }
        }
    }
}