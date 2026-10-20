using System;
using UnityEngine;

namespace Game.Combat
{
    /// <summary>
    /// Хранит, открыто ли сейчас окно парирования, и резолвит входящую атаку.
    /// Время НЕ отсчитывает — этим занимается ParryState (через Tick), поэтому
    /// корутин здесь нет, а длительности парирования не расходятся с состоянием игрока.
    /// </summary>
    public class ParryResolver : MonoBehaviour
    {
        [SerializeField] private float parryWindowDuration = 0.3f;
        [SerializeField] private float recoveryDuration = 0.2f;

        public float WindowDuration => parryWindowDuration;
        public float RecoveryDuration => recoveryDuration;
        public bool IsWindowOpen { get; private set; }

        /// <summary>Входящая атака была успешно парирована (для анимации/звука/VFX).</summary>
        public event Action OnParrySucceeded;

        public void OpenWindow()
        {
            IsWindowOpen = true;
            Debug.Log("ParryResolver: window OPEN");
        }

        public void CloseWindow()
        {
            if (IsWindowOpen) Debug.Log("ParryResolver: window CLOSED");
            IsWindowOpen = false;
        }

        /// <summary>Вызывается DamageReceiver при входящей атаке. true — атака парирована.</summary>
        public bool TryResolveIncomingAttack(DamagePacket packet)
        {
            if (!IsWindowOpen) return false;

            if (packet.AttackData == null || !packet.AttackData.canBeParried)
            {
                Debug.Log($"ParryResolver: атака от {packet.Source?.name} не парируется (CanBeParried = false)");
                return false;
            }

            IsWindowOpen = false; // одно окно — одно парирование
            Debug.Log($"ParryResolver: SUCCESS против {packet.Source?.name}");

            DamageReceiver attackerReceiver = packet.Source != null
                ? packet.Source.GetComponentInParent<DamageReceiver>()
                : null;

            if (attackerReceiver != null)
            {
                attackerReceiver.ReceiveStagger(packet.AttackData.staggerPower);
                attackerReceiver.ReceiveParry();
            }

            OnParrySucceeded?.Invoke();
            return true;
        }
    }
}
