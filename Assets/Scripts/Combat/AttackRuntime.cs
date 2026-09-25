using System.Collections.Generic;
using Game.Items;

namespace Game.Combat
{
    public enum AttackPhase
    {
        Startup,
        Active,
        Recovery,
        Done
    }

    /// <summary>
    /// Состояние одной конкретной выполняемой атаки. Нужен, чтобы одна
    /// атака не наносила одной и той же цели урон более одного раза.
    /// </summary>
    public class AttackRuntime
    {
        public AttackData AttackData { get; }
        public float StartTime { get; }
        public float DamageMultiplier { get; }
        public AttackPhase CurrentPhase { get; set; }
        public HashSet<DamageReceiver> AlreadyHitTargets { get; }

        public AttackRuntime(AttackData attackData, float startTime, float damageMultiplier = 1f)
        {
            AttackData = attackData;
            StartTime = startTime;
            DamageMultiplier = damageMultiplier;
            CurrentPhase = AttackPhase.Startup;
            AlreadyHitTargets = new HashSet<DamageReceiver>();
        }
    }
}
