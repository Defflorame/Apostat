using UnityEngine;
using Game.Items;
using Game.Magic;
using Game.Player;
using Game.Player.Equipment;

namespace Game.Combat
{
    /// <summary>
    /// Координатор боя игрока. Не рассчитывает урон самостоятельно.
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

        [Header("Magic")]
        [SerializeField] private SpellCaster spellCaster;
        [SerializeField] private RingSpellController ringSpellController;

        [Header("Equipment")]
        [SerializeField] private CombatLoadout combatLoadout;
        [SerializeField] private PlayerEquipment playerEquipment;

        [Header("Attack tuning")]
        [Tooltip("Порог удержания ЛКМ: короче — Light Attack, дольше — начинается заряд (Heavy Attack).")]
        [SerializeField] private float chargeHoldThreshold = 0.25f;

        [Header("Dodge tuning")]
        [SerializeField] private float dodgeStaminaCost = 20f;
        [SerializeField] private float dodgeDuration = 0.3f;

        [Header("Parry tuning")]
        [SerializeField] private float parryStaminaCost = 10f;

        private FreeState _freeState;
        private AttackState _attackState;
        private ChargeAttackState _chargeAttackState;
        private BlockState _blockState;
        private DodgeState _dodgeState;
        private ParryState _parryState;
        private CastState _castState;

        private bool _attackButtonHeld;
        private float _attackPressTime;
        private bool _chargeStarted;

        private void Awake()
        {
            _freeState = new FreeState();
            _attackState = new AttackState(weaponAttackController, ReturnToFree);
            _chargeAttackState = new ChargeAttackState(weaponAttackController, TrySpendStamina, ReturnToFree);
            _blockState = new BlockState(blockResolver, combatLoadout);
            _dodgeState = new DodgeState(playerMovement, combatLoadout, dodgeDuration, ReturnToFree);
            _parryState = new ParryState(parryResolver, ReturnToFree);
            _castState = new CastState(spellCaster, ringSpellController, ReturnToFree);
        }

        private void Start()
        {
            stateMachine.ChangeState(_freeState);
        }

        private void Update()
        {
            if (!_attackButtonHeld || _chargeStarted) return;

            if (!CanPerformAction())
            {
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
            if (combatLoadout.HasShield)
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
                Debug.Log("PlayerCombatController: Dodge недоступен в воздухе");
                return;
            }

            if (!TrySpendStamina(dodgeStaminaCost))
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

            if (!TrySpendStamina(parryStaminaCost))
            {
                Debug.Log("PlayerCombatController: недостаточно stamina для Parry");
                return;
            }

            stateMachine.ChangeState(_parryState);
        }

        /// <summary>Каст заклинания активного кольца. Стоимость списывает SpellCaster при входе в CastState.</summary>
        public void TryCastSpell()
        {
            if (!CanPerformAction())
            {
                Debug.Log("PlayerCombatController: Cast проигнорирован — текущее состояние не прерываемо");
                return;
            }

            SpellData spell = ringSpellController.ActiveSpell;
            if (spell == null)
            {
                Debug.Log("PlayerCombatController: Cast проигнорирован — нет кольца с заклинанием");
                return;
            }

            if (!spellCaster.CanCast(spell)) return;

            stateMachine.ChangeState(_castState);
        }

        /// <summary>direction: +1 следующее кольцо, -1 предыдущее. Переключение разрешено в любом состоянии.</summary>
        public void SwitchRing(int direction)
        {
            ringSpellController.SwitchRing(direction);
        }

        /// <summary>Переключает активную руку при двух одноручных оружиях (dual-wield).</summary>
        public void TrySwitchWeapon()
        {
            if (!CanPerformAction())
            {
                Debug.Log("PlayerCombatController: Switch weapon проигнорирован — текущее состояние не прерываемо");
                return;
            }

            combatLoadout.TrySwitchActiveWeapon();
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

            if (!lightAttackData)
            {
                Debug.Log("PlayerCombatController: Light Attack проигнорирован — игрок безоружен");
                return;
            }

            if (!TrySpendStamina(lightAttackData.staminaCost))
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

        /// <summary>Единственное место, где игрок тратит стамину на действие: базовая цена × множитель нагрузки экипировки.</summary>
        private bool TrySpendStamina(float baseCost)
        {
            if (stamina == null) return true;
            return stamina.TryConsume(baseCost * playerEquipment.GetStaminaCostMultiplier());
        }

        /// <summary>Оборвать всё, что игрок делает сейчас (смерть, позже — оглушение).</summary>
        public void CancelAllActions()
        {
            _attackButtonHeld = false;
            _chargeStarted = false;

            weaponAttackController.CancelAttack(); // если шла атака — сам вернёт во Free
            if (stateMachine.CurrentState != _freeState)
            {
                stateMachine.ChangeState(_freeState); // Cast / Block / Dodge / Parry / Charge
            }
        }
    }
}