using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Player
{
    /// <summary>
    /// Отвечает только за переключение Action Maps (Player / UI).
    /// Не содержит игровой логики.
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private InputActionAsset inputActions;

        public InputActionAsset InputActions => inputActions;

        private InputActionMap _playerMap;
        private InputActionMap _uiMap;

        private void Awake()
        {
            _playerMap = inputActions.FindActionMap("Player", throwIfNotFound: true);
            _uiMap = inputActions.FindActionMap("UI", throwIfNotFound: true);
        }

        private void OnEnable()
        {
            SwitchToPlayer();
        }

        private void OnDisable()
        {
            DisableAllInput();
        }

        public void SwitchToPlayer()
        {
            _uiMap.Disable();
            _playerMap.Enable();
        }

        public void SwitchToUI()
        {
            _playerMap.Disable();
            _uiMap.Enable();
        }

        public void DisableAllInput()
        {
            _playerMap.Disable();
            _uiMap.Disable();
        }
    }
}
