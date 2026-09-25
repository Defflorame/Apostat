using UnityEngine;
using Game.Player.Stats;

namespace Game.Player
{
    /// <summary>
    /// Горизонтальное перемещение игрока. Не смешивается с боевой системой.
    /// Ускоренный бег отсутствует. С Phase 3 базовая скорость берётся из
    /// CharacterStats (характеристика Speed) вместо константы в инспекторе,
    /// а EncumbranceService учитывает нагрузку экипировки — пока Equipment
    /// не реализован (Phase 4), currentWeight всегда 0, поэтому множитель
    /// нагрузки сейчас всегда 1 (нет видимого эффекта, но система готова).
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private CharacterStats characterStats;
        [SerializeField] private float gravity = -20f;
        [SerializeField] private float jumpHeight = 1.2f;

        private CharacterController _controller;
        private Vector2 _currentInput;
        private float _verticalVelocity;
        private bool _movementLocked;
        private float _effectiveSpeed;

        private bool _isDashing;
        private Vector3 _dashDirection;
        private float _dashSpeed;

        public bool IsGrounded => _controller.isGrounded;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
        }

        private void OnEnable()
        {
            if (characterStats != null)
            {
                characterStats.OnStatsChanged += HandleStatsChanged;
            }
        }

        private void OnDisable()
        {
            if (characterStats != null)
            {
                characterStats.OnStatsChanged -= HandleStatsChanged;
            }
        }

        private void Start()
        {
            // Читаем CharacterStats.Stats в Start(), а не в Awake(): порядок
            // выполнения Awake() между разными компонентами не гарантирован,
            // а Start() гарантированно выполняется после всех Awake().
            RecalculateSpeed();
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
                horizontalVelocity = moveDirection * _effectiveSpeed;
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

        private void HandleStatsChanged(DerivedCharacterStats stats)
        {
            RecalculateSpeed();
        }

        private void RecalculateSpeed()
        {
            if (characterStats == null)
            {
                Debug.LogWarning("PlayerMovement: CharacterStats не назначен — используется скорость по умолчанию.", this);
                _effectiveSpeed = 5f;
                return;
            }

            // TODO (Phase 4): currentWeight должен приходить из PlayerEquipment.GetTotalWeight().
            float currentWeight = 0f;
            float loadRatio = EncumbranceService.GetLoadRatio(currentWeight, characterStats.Stats.CarryWeightLimit);
            _effectiveSpeed = characterStats.Stats.MovementSpeed * EncumbranceService.GetMovementMultiplier(loadRatio);
        }
    }
}
