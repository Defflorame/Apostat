using UnityEngine;

namespace Game.Player.Stats
{
    /// <summary>
    /// Чистые функции преобразования нагрузки экипировки в игровые
    /// множители. Не хранит состояние (см. раздел 79 — "Service" для
    /// stateless-систем) — currentWeight и weightLimit передаёт вызывающая
    /// сторона. Сервис уже полностью рабочий и готов к реальным значениям веса.
    /// </summary>
    public static class EncumbranceService
    {
                private const float MinMovementMultiplier = 0.5f;
        private const float MinAttackSpeedMultiplier = 0.6f;
        private const float MaxStaminaCostMultiplier = 1.5f;

        /// <summary>До этой доли лимита штрафы не применяются вообще</summary>
        private const float FreeThreshold = 0.5f;

        /// <summary>0 — нет нагрузки, 1 — вес экипировки равен лимиту или превышает его.</summary>
        public static float GetLoadRatio(float currentWeight, float weightLimit)
        {
            if (weightLimit <= 0f) return 0f;
            return Mathf.Clamp01(currentWeight / weightLimit);
        }

        /// <summary>Перегоняет loadRatio в "штрафуемую" долю: 0 до FreeThreshold включительно, дальше линейно растёт к 1.</summary>
        private static float GetPenaltyRatio(float loadRatio)
        {
            if (loadRatio <= FreeThreshold) return 0f;
            return (loadRatio - FreeThreshold) / (1f - FreeThreshold);
        }

        public static float GetMovementMultiplier(float loadRatio)
        {
            return Mathf.Lerp(1f, MinMovementMultiplier, GetPenaltyRatio(loadRatio));
        }

        public static float GetAttackSpeedMultiplier(float loadRatio)
        {
            return Mathf.Lerp(1f, MinAttackSpeedMultiplier, GetPenaltyRatio(loadRatio));
        }

        public static float GetStaminaCostMultiplier(float loadRatio)
        {
            return Mathf.Lerp(1f, MaxStaminaCostMultiplier, GetPenaltyRatio(loadRatio));
        }
    }
}
