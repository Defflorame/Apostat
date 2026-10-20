namespace Game.Combat
{
    /// <summary>Runtime-состояние одного наложенного эффекта (StatusEffectData + таймеры).</summary>
    public class StatusEffectInstance
    {
        public StatusEffectData Data { get; }
        public float RemainingTime { get; set; }
        public float TimeToNextTick { get; set; }

        public StatusEffectInstance(StatusEffectData data)
        {
            Data = data;
            RemainingTime = data.duration;
            TimeToNextTick = data.tickInterval;
        }
    }
}
