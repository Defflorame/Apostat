using UnityEngine;

namespace Game.Player
{
    /// <summary>
    /// Mouse look: горизонтальный поворот тела персонажа, вертикальный —
    /// только камеры, с ограничением угла обзора.
    /// </summary>
    public class PlayerLook : MonoBehaviour
    {
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private Transform playerBody;
        [SerializeField] private float sensitivity = 0.1f;
        [SerializeField] private float minPitch = -80f;
        [SerializeField] private float maxPitch = 80f;

        private float _pitch;
        private bool _lookEnabled = true;

        public void Look(Vector2 input)
        {
            if (!_lookEnabled) return;

            float yaw = input.x * sensitivity;
            float pitchDelta = input.y * sensitivity;

            playerBody.Rotate(Vector3.up * yaw);

            _pitch = Mathf.Clamp(_pitch - pitchDelta, minPitch, maxPitch);
            cameraTransform.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
        }

        public void SetLookEnabled(bool enabled)
        {
            _lookEnabled = enabled;
        }

        public void ResetView()
        {
            _pitch = 0f;
            cameraTransform.localRotation = Quaternion.identity;
        }
    }
}
