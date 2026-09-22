using UnityEngine;
using Game.Items;
using Game.Player;

namespace Game.Combat
{
    /// <summary>
    /// Координатор боя игрока. Не рассчитывает урон самостоятельно.
    /// Отвечает за то, можно ли сейчас начать новое действие (стамина +
    /// текущее состояние), и передаёт команду в нужный State.
    /// </summary>
    public class PlayerCombatController : MonoBehaviour
    {
        [Header("Core")]
        [SerializeField] private PlayerActionStateMachine stateMachine;
        [SerializeField] private WeaponAttackController weaponAttackController;
        [SerializeField] private PlayerMovement playerMovement;
        [SerializeField] private StaminaComponent stamina;
        [SerializeField] private BlockResolver blockResolver;
        [SerializeField] private ParryResolver parryResolver;

        [Header("Attack tuning")]
        [Tooltip("Порог удержания ЛКМ: короче — Light Attack, дольше — начинается заряд (Heavy Attack).")]
        [SerializeField] private float chargeHoldThreshold = 0.25f;

        [Header("Dodge tuning")]
        [SerializeField] private float dodgeStaminaCost = 20f;
        [SerializeField] private float dodgeDuration = 0.3f;

        [Header("Parry tuning")]
        [SerializeField] private float parryStaminaCost = 10f;

        [Header("Временная заглушка — заменится PlayerEquipment (Phase 4)")]
        [Tooltip("Пока нет экипировки: вручную задаёт, есть ли щит. Если false — RMB выполняет Dodge вместо Block.")]
        [SerializeField] private bool hasShieldEquipped;

        private FreeState _freeState;
        private AttackState _attackState;
        private ChargeAttackState _chargeAttackState;
        private BlockState _blockState;
        private DodgeState _dodgeState;
        private ParryState _parryState;

        private bool _attackButtonHeld;
        private float _attackPressTime;
        private bool _chargeStarted;

        private void Awake()
        {
            _freeState = new FreeState();
            _attackState = new AttackState(weaponAttackController, ReturnToFree);

            AttackData heavyAttackData = weaponAttackController.HeavyAttackData;
            float maxChargeTime = heavyAttackData != null ? heavyAttackData.maxChargeDuration : 1f;
            _chargeAttackState = new ChargeAttackState(weaponAttackController, stamina, maxChargeTime, ReturnToFree);
            _blockState = new BlockState(blockResolver);

            float dodgeDistance = weaponAttackController.EquippedWeapon != null
                ? weaponAttackController.EquippedWeapon.dodgeDistance
                : 0f;
            float dodgeSpeed = dodgeDuration > 0f ? dodgeDistance / dodgeDuration : 0f;
            _dodgeState = new DodgeState(playerMovement, dodgeSpeed, dodgeDuration, ReturnToFree);

            _parryState = new ParryState(parryResolver, ReturnToFree);
        }

        private void Start()
        {
            stateMachine.ChangeState(_freeState);
        }

        private void Update()
        {
            // Момент перехода "быстрый тап" -> "начали заряжать".
            if (!_attackButtonHeld || _chargeStarted) return;

            if (!CanPerformAction())
            {
                // Текущее состояние сменилось (Parry/Dodge/Block) уже после нажатия ЛКМ —
                // сбрасываем отслеживание удержания, чтобы ни заряд, ни последующий
                // Light Attack по отпусканию кнопки не прервали непрерываемое состояние.
                _attackButtonHeld = false;
                return;
            }

            if (Time.time - _attackPressTime >= chargeHoldThreshold)
            {
                _chargeStarted = true;
                Debug.Log("PlayerCombatController: удержание превысило порог — переход в ChargeAttackState");
                stateMachine.ChangeState(_chargeAttackState);
            }
        }

        public void OnAttackPressed()
        {
            if (!CanPerformAction())
            {
                Debug.Log("PlayerCombatController: Attack press проигнорирован — текущее состояние не прерываемо");
                return;
            }

            _attackButtonHeld = true;
            _attackPressTime = Time.time;
            _chargeStarted = false;
        }

        public void OnAttackReleased()
        {
            if (!_attackButtonHeld) return;
            _attackButtonHeld = false;

            if (_chargeStarted)
            {
                _chargeAttackState.NotifyReleased();
            }
            else
            {
                TryLightAttack();
            }
        }

        public void StartBlockOrDodge()
        {
            if (hasShieldEquipped)
            {
                if (!CanPerformAction())
                {
                    Debug.Log("PlayerCombatController: Block проигнорирован — текущее состояние не прерываемо");
                    return;
                }

                stateMachine.ChangeState(_blockState);
            }
            else
            {
                TryDodge();
            }
        }

        public void StopBlock()
        {
            if (stateMachine.CurrentState == _blockState)
            {
                stateMachine.ChangeState(_freeState);
            }
        }

        public void TryDodge()
        {
            if (!CanPerformAction())
            {
                Debug.Log("PlayerCombatController: Dodge проигнорирован — текущее состояние не прерываемо");
                return;
            }

            if (!playerMovement.IsGrounded)
            {
                Debug.Log("PlayerCombatController: Parry недоступен в воздухе");
                return;
            }

            if (stamina != null && !stamina.TryConsume(dodgeStaminaCost))
            {
                Debug.Log("PlayerCombatController: недостаточно stamina для Dodge");
                return;
            }

            stateMachine.ChangeState(_dodgeState);
        }

        public void TryParry()
        {
            if (!CanPerformAction())
            {
                Debug.Log("PlayerCombatController: Parry проигнорирован — текущее состояние не прерываемо");
                return;
            }

            if (stamina != null && !stamina.TryConsume(parryStaminaCost))
            {
                Debug.Log("PlayerCombatController: недостаточно stamina для Parry");
                return;
            }

            stateMachine.ChangeState(_parryState);
        }

        public bool CanPerformAction()
        {
            return stateMachine.CurrentState == null || stateMachine.CurrentState.CanInterrupt();
        }

        private void TryLightAttack()
        {
            if (!CanPerformAction())
            {
                Debug.Log("PlayerCombatController: Light Attack проигнорирован — текущее состояние не прерываемо");
                return;
            }

            AttackData lightAttackData = weaponAttackController.LightAttackData;

            if (stamina != null && lightAttackData != null && !stamina.TryConsume(lightAttackData.staminaCost))
            {
                Debug.Log("PlayerCombatController: недостаточно stamina для Light Attack");
                return;
            }

            stateMachine.ChangeState(_attackState);
        }

        private void ReturnToFree()
        {
            stateMachine.ChangeState(_freeState);
        }
    }
}
