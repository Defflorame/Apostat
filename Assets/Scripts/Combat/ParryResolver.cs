using System;
using System.Collections;
using UnityEngine;

namespace Game.Combat
{
    /// <summary>
    /// Управляет окном парирования и резолвит, была ли входящая атака
    /// парирована вовремя. Настоящую пользу это принесёт, когда появятся
    /// атаки врагов (Phase 8) — сейчас проверяется через TestAttacker
    /// (см. Assets/Scripts/Debugging/TestAttacker.cs).
    /// </summary>
    public class ParryResolver : MonoBehaviour
    {
        [SerializeField] private float parryWindowDuration = 0.3f;
        [SerializeField] private float recoveryDuration = 0.2f;

        public event Action OnParryAttemptFinished;

        private bool _windowOpen;
        private Coroutine _routine;

        public void BeginParryAttempt()
        {
            if (_routine != null)
            {
                StopCoroutine(_routine);
            }

            _routine = StartCoroutine(RunWindow());
        }

        public void CancelParryAttempt()
        {
            if (_routine != null)
            {
                StopCoroutine(_routine);
                _routine = null;
            }

            _windowOpen = false;
        }

        /// <summary>
        /// Вызывается DamageReceiver при входящей атаке. Возвращает true,
        /// если атака была успешно парирована.
        /// </summary>
        public bool TryResolveIncomingAttack(DamagePacket packet)
        {
            if (!_windowOpen)
            {
                return false;
            }

            if (packet.AttackData == null || !packet.AttackData.canBeParried)
            {
                Debug.Log($"ParryResolver: атака от {packet.Source?.name} не парируется (CanBeParried = false)");
                return false;
            }

            _windowOpen = false;
            Debug.Log($"ParryResolver: SUCCESS против {packet.Source?.name}");

            DamageReceiver attackerReceiver = packet.Source != null
                ? packet.Source.GetComponentInParent<DamageReceiver>()
                : null;

            if (attackerReceiver == null)
            {
                Debug.Log("ParryResolver: у атакующего нет DamageReceiver — стаггер не применён (это нормально, пока нет Enemy AI).");
            }
            else
            {
                attackerReceiver.ReceiveStagger(packet.AttackData.staggerPower);
                attackerReceiver.ReceiveParry();
            }

            return true;
        }

        private IEnumerator RunWindow()
        {
            _windowOpen = true;
            Debug.Log("ParryResolver: window OPEN");

            yield return new WaitForSeconds(parryWindowDuration);

            _windowOpen = false;
            Debug.Log("ParryResolver: window CLOSED");

            yield return new WaitForSeconds(recoveryDuration);

            _routine = null;
            OnParryAttemptFinished?.Invoke();
        }
    }
}
