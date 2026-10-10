using UnityEngine;
using Game.Combat;
using Game.Items;
using Game.Player.Equipment;

namespace Game.Player.ViewModel
{
    /// <summary>
    /// Только отображает: подписывается на события боя и экипировки и
    /// выставляет параметры двух Animator (правая и левая рука). Игровых
    /// решений не принимает; боевые классы про него не знают.
    /// Параметры — только bool/int/float, отражающие текущее состояние игры,
    /// поэтому зависших триггеров быть не может.
    /// </summary>
    public class PlayerAnimator : MonoBehaviour
    {
        [Header("Источники событий")]
        [SerializeField] private PlayerActionStateMachine stateMachine;
        [SerializeField] private WeaponAttackController attackController;
        [SerializeField] private CombatLoadout combatLoadout;

        [Header("Правая рука")]
        [SerializeField] private Animator rightAnimator;
        [Tooltip("Контроллер без оружия (кулаки).")]
        [SerializeField] private RuntimeAnimatorController rightDefaultController;

        [Header("Левая рука")]
        [SerializeField] private Animator leftAnimator;
        [Tooltip("Контроллер без щита.")]
        [SerializeField] private RuntimeAnimatorController leftDefaultController;

        private readonly float[] _lightClipLengths = new float[PlayerAnimatorIds.LightAttackClips.Length];
        private float _heavyClipLength;

        private void OnEnable()
        {
            stateMachine.OnStateChanged += HandleStateChanged;
            attackController.OnAttackStarted += HandleAttackStarted;
            combatLoadout.OnLoadoutChanged += RefreshControllers;
        }

        private void OnDisable()
        {
            stateMachine.OnStateChanged -= HandleStateChanged;
            attackController.OnAttackStarted -= HandleAttackStarted;
            combatLoadout.OnLoadoutChanged -= RefreshControllers;
        }

        private void Start()
        {
            RefreshControllers();
        }

        private void RefreshControllers()
        {
            WeaponData weapon = combatLoadout.CurrentWeapon;
            ShieldData shield = combatLoadout.CurrentShield;

            RuntimeAnimatorController right = weapon != null && weapon.animatorOverride != null
                ? weapon.animatorOverride
                : rightDefaultController;
            RuntimeAnimatorController left = shield != null && shield.animatorOverride != null
                ? shield.animatorOverride
                : leftDefaultController;

            AssignController(rightAnimator, right);
            AssignController(leftAnimator, left);

            for (int i = 0; i < _lightClipLengths.Length; i++)
            {
                _lightClipLengths[i] = GetClipLength(right, PlayerAnimatorIds.LightAttackClips[i]);
            }
            _heavyClipLength = GetClipLength(right, PlayerAnimatorIds.HeavyAttackClip);

            // Смена контроллера сбрасывает параметры — восстановить по текущему состоянию.
            ApplyState(stateMachine.CurrentState);
        }

        private void HandleStateChanged(IPlayerActionState state)
        {
            ApplyState(state);
        }

        private void ApplyState(IPlayerActionState state)
        {
            bool isCharge = state is ChargeAttackState;
            bool isAttack = state is AttackState;

            SetBool(rightAnimator, PlayerAnimatorIds.IsCharging, isCharge);
            SetBool(rightAnimator, PlayerAnimatorIds.IsDodging, state is DodgeState);
            SetBool(rightAnimator, PlayerAnimatorIds.IsParrying, state is ParryState);
            SetBool(leftAnimator, PlayerAnimatorIds.IsBlocking, state is BlockState);

            // Флаги атаки выставляет OnAttackStarted; здесь только сбрасываем при выходе из атаки.
            if (!isAttack && !isCharge)
            {
                SetBool(rightAnimator, PlayerAnimatorIds.IsLightAttacking, false);
                SetBool(rightAnimator, PlayerAnimatorIds.IsHeavyAttacking, false);
            }
            else if (isCharge)
            {
                SetBool(rightAnimator, PlayerAnimatorIds.IsHeavyAttacking, false);
            }
        }

        private void HandleAttackStarted(AttackData attackData, float damageMultiplier)
        {
            bool isHeavy = attackData.attackType == AttackType.Heavy;
            int variant = isHeavy ? 0 : Mathf.Clamp(attackData.animationVariant, 0, _lightClipLengths.Length - 1);

            float clipLength = isHeavy ? _heavyClipLength : _lightClipLengths[variant];
            float totalDuration = attackData.startupDuration + attackData.activeDuration + attackData.recoveryDuration;
            float speed = clipLength > 0f && totalDuration > 0f ? clipLength / totalDuration : 1f;

            if (rightAnimator.isActiveAndEnabled)
            {
                rightAnimator.SetFloat(PlayerAnimatorIds.AttackSpeed, speed);
            }

            if (isHeavy)
            {
                SetBool(rightAnimator, PlayerAnimatorIds.IsCharging, false);
                SetBool(rightAnimator, PlayerAnimatorIds.IsHeavyAttacking, true);
            }
            else
            {
                if (rightAnimator.isActiveAndEnabled)
                {
                    rightAnimator.SetInteger(PlayerAnimatorIds.LightVariant, variant);
                }
                SetBool(rightAnimator, PlayerAnimatorIds.IsLightAttacking, true);
            }
        }

        private static void AssignController(Animator animator, RuntimeAnimatorController controller)
        {
            if (animator.runtimeAnimatorController == controller) return;
            animator.runtimeAnimatorController = controller;
        }

        private static void SetBool(Animator animator, int id, bool value)
        {
            // На скрытой руке (выключенный GameObject) Animator параметры не принимает.
            if (animator != null && animator.isActiveAndEnabled)
            {
                animator.SetBool(id, value);
            }
        }

        /// <summary>Длина клипа-слота: из оверрайда по имени исходного клипа или из самого контроллера.</summary>
        private static float GetClipLength(RuntimeAnimatorController controller, string clipName)
        {
            if (controller == null) return 0f;

            if (controller is AnimatorOverrideController overrideController)
            {
                AnimationClip overridden = overrideController[clipName];
                return overridden != null ? overridden.length : 0f;
            }

            foreach (AnimationClip clip in controller.animationClips)
            {
                if (clip.name == clipName) return clip.length;
            }

            return 0f;
        }
    }
}