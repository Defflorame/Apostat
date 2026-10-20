using UnityEngine;
using Game.Combat;

namespace Game.Magic
{
    public enum SpellResource
    {
        Mana,
        Health
    }

    /// <summary>
    /// Описывает, чем заклинание ЯВЛЯЕТСЯ: стоимость, тайминги и список эффектов.
    /// Runtime state (кулдауны) живёт в SpellCaster. Что именно делает заклинание,
    /// определяют ассеты SpellEffectData в списке effects.
    /// </summary>
    [CreateAssetMenu(fileName = "NewSpellData", menuName = "Game/Magic/Spell Data")]
    public class SpellData : ScriptableObject
    {
        public string spellName;
        [TextArea] public string description;

        [Header("Cost")]
        public SpellResource resource = SpellResource.Mana;
        [Tooltip("Сколько маны или HP тратится за каст. Списывается в момент выпуска заклинания (для мгновенных — при нажатии).")]
        public int cost = 10;
        [Tooltip("Только для resource = Health. true — потраченное HP постепенно возвращается само; false — HP потеряно, пока его не вылечат другим способом.")]
        public bool healthCostRecovers = false;

        [Header("Timing (seconds)")]
        [Tooltip("Кулдаун стартует в момент выпуска заклинания.")]
        public float cooldown = 0.5f;
        [Tooltip("Минимальное время удержания кнопки. После него заряд готов и держится сколько угодно. 0 = МГНОВЕННЫЙ каст: срабатывает при нажатии, без удержания.")]
        public float castTime = 0.2f;
        [Tooltip("Время после выпуска, в течение которого игрок ещё не может действовать.")]
        public float recoveryTime = 0.3f;

        [Header("Effects")]
        [Tooltip("Эффекты, которые срабатывают при выпуске заклинания, по порядку.")]
        public SpellEffectData[] effects;

        [Header("Elemental damage (для эффектов, наносящих урон)")]
        [Tooltip("Стихия урона заклинания. Пусто — нейтральная магия (на неё действует только magicResistance цели).")]
        public ElementType element;

        [Header("Status effects on hit")]
        [Tooltip("Эффекты, которые урон заклинания пытается наложить на цель (например, яд от облака). Не путать с Buff-эффектом, который накладывается на самого заклинателя.")]
        public StatusEffectApplication[] onHitEffects;

        /// <summary>castTime = 0 — каст без удержания.</summary>
        public bool IsInstant => castTime <= 0f;

        private void OnValidate()
        {
            cooldown = Mathf.Max(0f, cooldown);
            castTime = Mathf.Max(0f, castTime);
            recoveryTime = Mathf.Max(0f, recoveryTime);
        }
    }
}
