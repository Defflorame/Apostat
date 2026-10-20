using System;
using UnityEngine;

namespace Game.Combat
{
    /// <summary>
    /// "Накладывает эффект X с шансом Y при попадании".
    /// Один и тот же формат используют WeaponData (оружие, позже враги) и SpellData (заклинания).
    /// </summary>
    [Serializable]
    public class StatusEffectApplication
    {
        public StatusEffectData effect;

        [Range(0f, 100f)]
        [Tooltip("Шанс наложения эффекта, %. 0 — никогда, 100 — всегда. Дальше уменьшается сопротивлением цели этому эффекту (ResistanceProfileData).")]
        public float chancePercent = 100f;
    }
}