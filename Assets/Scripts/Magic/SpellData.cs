using UnityEngine;

namespace Game.Magic
{
    public enum SpellResource
    {
        Mana,
        Health
    }

    /// <summary>
    /// Описывает, чем заклинание ЯВЛЯЕТСЯ. Не хранит runtime state
    /// (кулдауны живут в SpellCaster).
    /// </summary>
    [CreateAssetMenu(fileName = "NewSpellData", menuName = "Game/Magic/Spell Data")]
    public class SpellData : ScriptableObject
    {
        public string spellName;
        [TextArea] public string description;

        [Header("Cost")]
        public SpellResource resource = SpellResource.Mana;
        [Tooltip("Сколько маны или HP тратится за каст.")]
        public int cost = 10;
        [Tooltip("Только для resource = Health. true — потраченное HP постепенно возвращается само; false — HP потеряно, пока его не вылечат другим способом.")]
        public bool healthCostRecovers = false;

        [Header("Timing (seconds)")]
        public float cooldown = 0.5f;
        [Tooltip("Время от начала каста до выпуска заклинания.")]
        public float castTime = 0.2f;
        [Tooltip("Время после выпуска, в течение которого игрок ещё не может действовать.")]
        public float recoveryTime = 0.3f;

        [Header("Projectile effect")]
        public SpellProjectile projectilePrefab;
        public int damage = 20;
        public float projectileSpeed = 20f;
        public float projectileLifetime = 3f;
    }
}