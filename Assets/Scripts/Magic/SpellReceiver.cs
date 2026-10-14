using System;
using UnityEngine;

namespace Game.Magic
{
    /// <summary>
    /// Объект мира, который может реагировать на заклинание. Сам не решает,
    /// что делать — подписчики (двери, иллюзорные стены в Phase 9) слушают событие.
    /// </summary>
    public class SpellReceiver : MonoBehaviour
    {
        public event Action<SpellData, GameObject> OnSpellReceived;

        public void Receive(SpellData spell, GameObject caster)
        {
            Debug.Log($"SpellReceiver: {name} получил заклинание {spell.spellName}");
            OnSpellReceived?.Invoke(spell, caster);
        }
    }
}