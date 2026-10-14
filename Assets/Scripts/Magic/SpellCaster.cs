using System;
using System.Collections.Generic;
using UnityEngine;
using Game.Combat;

namespace Game.Magic
{
    /// <summary>
    /// Проверяет и списывает стоимость заклинания, ведёт кулдауны.
    /// Не знает про кольца, ввод и состояния — получает SpellData снаружи.
    /// События нужны будущим PlayerAnimator/Audio/VFX.
    /// </summary>
    public class SpellCaster : MonoBehaviour
    {
        [SerializeField] private ManaComponent mana;
        [SerializeField] private HealthComponent health;
        [SerializeField] private SpentHealthRecovery spentHealthRecovery;
        [SerializeField] private SpellEffectExecutor effectExecutor;

        public event Action<SpellData> OnCastStarted;
        public event Action<SpellData> OnSpellReleased;

        private readonly Dictionary<SpellData, float> _readyAt = new Dictionary<SpellData, float>();

        public float GetCooldownRemaining(SpellData spell)
        {
            if (spell == null || !_readyAt.TryGetValue(spell, out float readyAt)) return 0f;
            return Mathf.Max(0f, readyAt - Time.time);
        }

        public bool CanCast(SpellData spell)
        {
            if (spell == null || health.IsDead()) return false;

            if (GetCooldownRemaining(spell) > 0f)
            {
                Debug.Log($"SpellCaster: {spell.spellName} на кулдауне");
                return false;
            }

            switch (spell.resource)
            {
                case SpellResource.Mana:
                    if (!mana.CanSpend(spell.cost))
                    {
                        Debug.Log($"SpellCaster: недостаточно маны для {spell.spellName}");
                        return false;
                    }
                    break;

                case SpellResource.Health:
                    // Заклинание за HP не может убить самого заклинателя.
                    if (health.CurrentHealth <= spell.cost)
                    {
                        Debug.Log($"SpellCaster: недостаточно HP для {spell.spellName}");
                        return false;
                    }
                    break;
            }

            return true;
        }

        /// <summary>Проверяет, списывает стоимость, запускает кулдаун. Эффект выпускается позже — ReleaseSpell.</summary>
        public bool BeginCast(SpellData spell)
        {
            if (!CanCast(spell)) return false;

            if (spell.resource == SpellResource.Mana)
            {
                mana.TryConsume(spell.cost);
            }
            else
            {
                health.SpendHealth(spell.cost);
                if (spell.healthCostRecovers && spentHealthRecovery != null)
                {
                    spentHealthRecovery.AddPending(spell.cost);
                }
            }

            _readyAt[spell] = Time.time + spell.cooldown;
            OnCastStarted?.Invoke(spell);
            return true;
        }

        public void ReleaseSpell(SpellData spell)
        {
            effectExecutor.Execute(spell);
            OnSpellReleased?.Invoke(spell);
        }
    }
}