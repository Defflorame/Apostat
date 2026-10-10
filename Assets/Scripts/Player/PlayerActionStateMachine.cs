using System;
using UnityEngine;

namespace Game.Player
{
    /// <summary>
    /// Отвечает за взаимоисключающие действия игрока (Free, Attack и т.д.).
    /// Передвижение и обзор сюда не входят — они работают параллельно
    /// с любым состоянием.
    /// </summary>
    public class PlayerActionStateMachine : MonoBehaviour
    {
        public IPlayerActionState CurrentState { get; private set; }

        /// <summary>
        /// Вызывается при каждой смене состояния ДО Enter() нового состояния.
        /// Так порядок событий остаётся верным, даже если Enter() сразу
        /// переключит состояние дальше.
        /// </summary>
        public event Action<IPlayerActionState> OnStateChanged;

        private void Update()
        {
            CurrentState?.Tick();
        }

        /// <summary>
        /// Переключает состояние безусловно. CanInterrupt() машина сама
        /// не проверяет — иначе состояние не сможет корректно завершить
        /// само себя (например, AttackState → FreeState по окончании атаки).
        /// Решение "можно ли прервать текущее действие новым" принимает
        /// вызывающий код (например PlayerCombatController.CanPerformAction()),
        /// ДО обращения к ChangeState.
        /// </summary>
        public void ChangeState(IPlayerActionState newState)
        {
            CurrentState?.Exit();
            CurrentState = newState;
            OnStateChanged?.Invoke(newState);
            CurrentState?.Enter();
        }

        public bool IsInState<T>() where T : IPlayerActionState
        {
            return CurrentState is T;
        }
    }
}