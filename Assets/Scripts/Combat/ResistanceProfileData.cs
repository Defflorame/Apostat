using System;
using UnityEngine;

namespace Game.Combat
{
    /// <summary>Урон конкретной стихии в процентах. 100 — норма, 0 — иммунитет, 200 — двойной урон.</summary>
    [Serializable]
    public class ElementResistanceEntry
    {
        public ElementType element;

        [Range(0f, 200f)]
        [Tooltip("Сколько % урона этой стихии получает юнит. 100 — обычный урон, 50 — вдвое меньше, 0 — иммунитет, 200 — вдвое больше (слабость).")]
        public float damagePercent = 100f;
    }

    /// <summary>Сопротивление конкретному статус-эффекту в процентах. 0 — нет защиты, 100 — иммунитет.</summary>
    [Serializable]
    public class StatusResistanceEntry
    {
        public StatusEffectData effect;

        [Range(0f, 100f)]
        [Tooltip("Сопротивление эффекту, %. 0 — эффект накладывается как обычно, 50 — шанс наложения вдвое ниже, 100 — иммунитет.")]
        public float resistancePercent = 0f;
    }

    /// <summary>
    /// Профиль защиты юнита (данные, не runtime state). Один ассет можно назначить многим юнитам.
    /// Всё, чего в профиле нет, считается нейтральным: сопротивление 0, урон стихии 100%.
    /// </summary>
    [CreateAssetMenu(fileName = "NewResistanceProfile", menuName = "Game/Combat/Resistance Profile")]
    public class ResistanceProfileData : ScriptableObject
    {
        [Range(0f, 100f)]
        [Tooltip("Сопротивление магическому урону (заклинания), %. 0 — нет защиты, 50 — вдвое меньше урона, 100 — иммунитет. Применяется ДО стихии.")]
        public float magicResistance = 0f;

        [Header("Physical")]
        [Range(0f, 100f)]
        [Tooltip("Сопротивление физическому урону, %. 0 — нет защиты, 50 — вдвое меньше урона, 100 — иммунитет.")]
        public float physicalResistance = 0f;

        [Header("Elements")]
        [Tooltip("Сколько % урона каждой стихии получает юнит (100 — норма). Стихия не указана — 100. Указана дважды — берётся первая запись.")]
        public ElementResistanceEntry[] elements;

        [Header("Status effects")]
        [Tooltip("Сопротивление статус-эффектам, %. Эффект не указан — 0 (защиты нет). Итоговый шанс = шанс атаки × (1 − сопротивление / 100).")]
        public StatusResistanceEntry[] statuses;

        public float GetMagicResistancePercent() => Mathf.Clamp(magicResistance, 0f, 100f);

        public float GetPhysicalResistancePercent() => Mathf.Clamp(physicalResistance, 0f, 100f);

        public float GetElementPercent(ElementType element)
        {
            if (element == null || elements == null) return 100f;

            for (int i = 0; i < elements.Length; i++)
            {
                if (elements[i] != null && elements[i].element == element)
                {
                    return Mathf.Clamp(elements[i].damagePercent, 0f, 200f);
                }
            }

            return 100f;
        }

        public float GetStatusResistancePercent(StatusEffectData effect)
        {
            if (effect == null || statuses == null) return 0f;

            for (int i = 0; i < statuses.Length; i++)
            {
                if (statuses[i] != null && statuses[i].effect == effect)
                {
                    return Mathf.Clamp(statuses[i].resistancePercent, 0f, 100f);
                }
            }

            return 0f;
        }
    }
}