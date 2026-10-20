using UnityEngine;

namespace Game.Magic
{
    /// <summary>
    /// Создаёт временную область урона в точке, на которую наведён прицел
    /// (первая поверхность на луче из точки каста). Нет поверхности в пределах
    /// maxPlaceDistance (смотрите в небо) — заклинание срывается без трат.
    /// Форма и размер зоны задаются коллайдерами префаба; масштаб префаба не перезаписывается.
    /// </summary>
    [CreateAssetMenu(fileName = "NewZoneEffect", menuName = "Game/Magic/Effects/Damage Zone")]
    public class ZoneEffectData : SpellEffectData
    {
        [Tooltip("Необязательно. Префаб зоны: форма области урона = его Collider'ы (Box/Sphere/Capsule), размер = scale префаба + размер коллайдера. Визуал — любые дочерние объекты. Без префаба зона невидимая и сферическая, радиуса radius.")]
        public DamageZone zonePrefab;

        public int damagePerTick = 8;
        public float tickInterval = 0.5f;
        [Tooltip("Радиус сферы, метры. Используется ТОЛЬКО если префаба нет или у него нет коллайдеров.")]
        public float radius = 3f;
        [Tooltip("Сколько секунд существует зона.")]
        public float duration = 5f;
        [Tooltip("Максимальная дальность прицеливания, метры.")]
        public float maxPlaceDistance = 20f;

        [Header("Defense")]
        [Tooltip("Может ли цель блокировать урон зоны щитом. Для зоны на земле обычно нет.")]
        public bool canBeBlocked = false;
        [Tooltip("Если включено, защита (Defense) цели не уменьшает урон тика зоны. Сопротивления (магия, стихии) продолжают работать.")]
        public bool tickIgnoresDefense = false;
        [Tooltip("Бьёт ли зона по самому заклинателю.")]
        public bool affectCaster = true;

        public override bool CanExecute(SpellCastContext context)
        {
            if (!SpellAim.TryRaycast(context, maxPlaceDistance, out _))
            {
                Debug.Log($"ZoneEffectData '{name}': нет поверхности в прицеле — зона не создана");
                return false;
            }

            return true;
        }

        public override void Execute(SpellCastContext context)
        {
            if (!SpellAim.TryRaycast(context, maxPlaceDistance, out RaycastHit hit)) return;

            // Небольшой отступ от поверхности, чтобы зона не лежала внутри пола или стены.
            Vector3 point = hit.point + hit.normal * 0.05f;

            DamageZone zone;
            if (zonePrefab != null)
            {
                zone = Instantiate(zonePrefab, point, Quaternion.identity);
            }
            else
            {
                zone = new GameObject($"{name}_Zone").AddComponent<DamageZone>();
                zone.transform.position = point;
            }

            zone.Initialize(this, context, context.ScaleDamage(damagePerTick));
        }
    }
}
