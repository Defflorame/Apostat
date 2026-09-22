using UnityEngine;

namespace Game.Player
{
    /// <summary>
    /// Горизонтальное перемещение игрока. Не смешивается с боевой системой.
    /// Ускоренный бег отсутствует; модификаторы скорости (нагрузка экипировки,
    /// характеристики) будут подключены на следующих этапах.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float baseSpeed = 5f;
        [SerializeField] private float gravity = -20f;
        [SerializeField] private float jumpHeight = 1.2f;

        private CharacterController _controller;
        private Vector2 _currentInput;
        private float _verticalVelocity;
        private bool _movementLocked;

        private bool _isDashing;
        private Vector3 _dashDirection;
        private float _dashSpeed;
        
        public bool IsGrounded => _controller.isGrounded;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
        }

        private void Update()
        {
            if (_movementLocked && !_isDashing)
            {
                _currentInput = Vector2.zero;
            }

            ApplyGravity();

            Vector3 horizontalVelocity;
            if (_isDashing)
            {
                horizontalVelocity = _dashDirection * _dashSpeed;
            }
            else
            {
                Vector3 moveDirection = transform.right * _currentInput.x + transform.forward * _currentInput.y;
                horizontalVelocity = moveDirection * baseSpeed;
            }

            Vector3 velocity = horizontalVelocity;
            velocity.y = _verticalVelocity;

            _controller.Move(velocity * Time.deltaTime);
        }

        private void ApplyGravity()
        {
            if (_controller.isGrounded && _verticalVelocity < 0f)
            {
                _verticalVelocity = -2f;
            }
            else
            {
                _verticalVelocity += gravity * Time.deltaTime;
            }
        }

        public void Move(Vector2 input)
        {
            _currentInput = input;
        }

        public void StopMovement()
        {
            _currentInput = Vector2.zero;
        }

        public Vector3 GetVelocity()
        {
            return _controller.velocity;
        }

        public void SetMovementLocked(bool locked)
        {
            _movementLocked = locked;
            if (locked)
            {
                _currentInput = Vector2.zero;
            }
        }

        /// <summary>Начинает управляемый рывок (используется DodgeState), игнорируя обычный ввод.</summary>
        public void BeginDash(Vector3 worldDirection, float speed)
        {
            _isDashing = true;
            _dashDirection = worldDirection.normalized;
            _dashSpeed = speed;
        }

        public void EndDash()
        {
            _isDashing = false;
        }

        public void Jump()
        {
            if (_movementLocked || !_controller.isGrounded) return;

            _verticalVelocity = Mathf.Sqrt(2f * Mathf.Abs(gravity) * jumpHeight);
        }
    }
}
