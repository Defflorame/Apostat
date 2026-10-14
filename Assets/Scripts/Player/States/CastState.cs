using System;
using UnityEngine;
using Game.Magic;

namespace Game.Player
{
    /// <summary>
    /// Каст заклинания активного кольца: windup → выпуск → recovery.
    /// Время идёт через Tick (без корутин). Во время каста игрок может
    /// двигаться, но не может начать другое боевое действие.
    /// </summary>
    public class CastState : IPlayerActionState
    {
        private readonly SpellCaster _spellCaster;
        private readonly RingSpellController _ringSpellController;
        private readonly Action _onFinished;

        private SpellData _spell;
        private float _elapsed;
        private bool _released;

        public CastState(SpellCaster spellCaster, RingSpellController ringSpellController, Action onFinished)
        {
            _spellCaster = spellCaster;
            _ringSpellController = ringSpellController;
            _onFinished = onFinished;
        }

        public void Enter()
        {
            _elapsed = 0f;
            _released = false;
            _spell = _ringSpellController.ActiveSpell;

            if (_spell == null || !_spellCaster.BeginCast(_spell))
            {
                _spell = null; // завершится в первом Tick
            }
        }

        public void Tick()
        {
            if (_spell == null)
            {
                _onFinished?.Invoke();
                return;
            }

            _elapsed += Time.deltaTime;

            if (!_released && _elapsed >= _spell.castTime)
            {
                _released = true;
                _spellCaster.ReleaseSpell(_spell);
            }

            if (_elapsed >= _spell.castTime + _spell.recoveryTime)
            {
                _onFinished?.Invoke();
            }
        }

        public void Exit() { }

        public bool CanInterrupt() => false;
    }
}