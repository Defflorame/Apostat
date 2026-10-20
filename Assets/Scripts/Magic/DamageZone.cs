using UnityEngine;

namespace Game.Magic
{
    /// <summary>
    /// Временная область урона на земле. Живёт duration секунд, каждые tickInterval
    /// секунд бьёт по всем целям внутри области (через SpellHitResolver).
    /// SpellReceiver уведомляются только на первом тике, а не на каждом.
    ///
    /// Форма области = коллайдеры префаба (Box/Sphere/Capsule на корне или детях).
    /// Если у префаба нет коллайдеров или префаба нет вовсе — используется сфера
    /// радиуса ZoneEffectData.radius.
    /// </summary>
    public class DamageZone : MonoBehaviour
    {
        private ZoneEffectData _data;
        private SpellData _spell;
        private GameObject _caster;
        private LayerMask _mask;
        private int _damagePerTick;
        private Collider[] _shapes = System.Array.Empty<Collider>();
        private float _elapsed;
        private float _nextTickTime;
        private bool _receiversNotified;

        public void Initialize(ZoneEffectData data, SpellCastContext context, int damagePerTick)
        {
            _data = data;
            _spell = context.Spell;
            _caster = context.Caster;
            _mask = context.AimMask;
            _damagePerTick = damagePerTick;

            _shapes = GetComponentsInChildren<Collider>();

            // Коллайдеры зоны — только форма области: они не должны мешать ходить и не должны быть целями лучей.
            foreach (Collider shape in _shapes)
            {
                shape.isTrigger = true;
            }

            if (_shapes.Length == 0 && data.zonePrefab != null)
            {
                Debug.LogWarning($"DamageZone: у префаба '{data.zonePrefab.name}' нет Collider — используется сфера радиуса {data.radius}.", this);
            }
        }

        private void Update()
        {
            if (_data == null) return;

            _elapsed += Time.deltaTime;

            if (_elapsed >= _nextTickTime)
            {
                Tick();
                _nextTickTime += Mathf.Max(0.05f, _data.tickInterval);
            }

            if (_elapsed >= _data.duration)
            {
                Destroy(gameObject);
            }
        }

        private void Tick()
        {
            bool notify = !_receiversNotified;

            if (_shapes.Length > 0)
            {
                SpellHitResolver.HitShapes(
                    _spell, _caster, _shapes,
                    _damagePerTick, _data.canBeBlocked, _data.affectCaster, _mask,
                    notifySpellReceivers: notify,
                    ignoreDefense: _data.tickIgnoresDefense);
            }
            else
            {
                SpellHitResolver.HitArea(
                    _spell, _caster, transform.position, _data.radius,
                    _damagePerTick, _data.canBeBlocked, _data.affectCaster, _mask,
                    notifySpellReceivers: notify,
                    ignoreDefense: _data.tickIgnoresDefense);
            }

            _receiversNotified = true;
        }

        private void OnDrawGizmos()
        {
            if (_data == null) return;

            Gizmos.color = new Color(1f, 0.3f, 0.1f, 0.6f);

            if (_shapes.Length == 0)
            {
                Gizmos.DrawWireSphere(transform.position, _data.radius);
                return;
            }

            foreach (Collider shape in _shapes)
            {
                if (shape != null) DrawShapeGizmo(shape);
            }
        }

        private static void DrawShapeGizmo(Collider shape)
        {
            Transform t = shape.transform;

            switch (shape)
            {
                case BoxCollider box:
                    Gizmos.matrix = Matrix4x4.TRS(t.position, t.rotation, t.lossyScale);
                    Gizmos.DrawWireCube(box.center, box.size);
                    break;

                case SphereCollider sphere:
                    Gizmos.matrix = Matrix4x4.TRS(t.position, t.rotation, t.lossyScale);
                    Gizmos.DrawWireSphere(sphere.center, sphere.radius);
                    break;

                default:
                    // Capsule и прочие: приближение по bounds (сам запрос урона для Capsule точный).
                    Gizmos.DrawWireCube(shape.bounds.center, shape.bounds.size);
                    break;
            }

            Gizmos.matrix = Matrix4x4.identity;
        }
    }
}
