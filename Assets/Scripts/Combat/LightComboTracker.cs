using UnityEngine;
using Game.Items;
using Game.Player.Equipment;

namespace Game.Combat
{
    /// <summary>
    /// Помнит, какая лёгкая атака серии идёт следующей. Единственная
    /// ответственность — порядок серии; сами атаки запускает WeaponAttackController.
    /// Серия сбрасывается, если после конца атаки прошло больше seriesWindow
    /// секунд, при смене оружия и после тяжёлой атаки.
    /// </summary>
    public class LightComboTracker : MonoBehaviour
    {
        [SerializeField] private CombatLoadout combatLoadout;
        [Tooltip("Сколько секунд после конца атаки серия остаётся активной.")]
        [SerializeField] private float seriesWindow = 1f;

        private int _nextIndex;
        // Пока атака идёт, окно не истекает.
        private float _windowEndsAt = float.PositiveInfinity;

        private void OnEnable()
        {
            combatLoadout.OnLoadoutChanged += Reset;
        }

        private void OnDisable()
        {
            combatLoadout.OnLoadoutChanged -= Reset;
        }

        /// <summary>Какая лёгкая атака будет следующей. Состояние не меняет (нужно для проверки стамины).</summary>
        public AttackData PeekNext()
        {
            WeaponData weapon = combatLoadout.CurrentWeapon;
            if (weapon == null) return null;

            return weapon.GetLightAttack(ResolveIndex(weapon));
        }

        /// <summary>Вызывается в момент старта лёгкой атаки: возвращает её и сдвигает серию.</summary>
        public AttackData BeginLightAttack()
        {
            WeaponData weapon = combatLoadout.CurrentWeapon;
            if (weapon == null) return null;

            int index = ResolveIndex(weapon);
            AttackData attack = weapon.GetLightAttack(index);

            _nextIndex = (index + 1) % Mathf.Max(1, weapon.LightAttackCount);
            _windowEndsAt = float.PositiveInfinity;
            return attack;
        }

        /// <summary>Вызывается при завершении (или отмене) любой атаки — с этого момента идёт отсчёт окна серии.</summary>
        public void NotifyAttackFinished()
        {
            _windowEndsAt = Time.time + seriesWindow;
        }

        public void Reset()
        {
            _nextIndex = 0;
            _windowEndsAt = float.PositiveInfinity;
        }

        private int ResolveIndex(WeaponData weapon)
        {
            if (Time.time > _windowEndsAt || _nextIndex >= weapon.LightAttackCount) return 0;
            return _nextIndex;
        }
    }
}