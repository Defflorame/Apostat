using System.Collections.Generic;
using UnityEngine;

namespace Game.Combat
{
    /// <summary>
    /// Временные и экипировочные бонусы к сопротивлению стихиям (проценты, + = сопротивление).
    /// Необязателен: без него юнит использует только ResistanceProfileData.
    /// Source нужен, чтобы снять именно свои бонусы (предмет, эффект).
    /// </summary>
    public class ElementResistanceModifiers : MonoBehaviour
    {
        private struct Bonus
        {
            public object Source;
            public ElementType Element;
            public float Percent;
        }

        private readonly List<Bonus> _bonuses = new List<Bonus>();

        public void Add(object source, ElementType element, float percent)
        {
            if (element == null || percent == 0f) return;
            _bonuses.Add(new Bonus { Source = source, Element = element, Percent = percent });
        }

        public void RemoveFromSource(object source)
        {
            _bonuses.RemoveAll(b => b.Source == source);
        }

        public float GetBonusPercent(ElementType element)
        {
            float sum = 0f;
            for (int i = 0; i < _bonuses.Count; i++)
            {
                if (_bonuses[i].Element == element) sum += _bonuses[i].Percent;
            }

            return Mathf.Clamp(sum, -100f, 100f);
        }
    }
}