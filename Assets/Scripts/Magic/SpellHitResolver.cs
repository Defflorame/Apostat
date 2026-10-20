using System.Collections.Generic;
using UnityEngine;
using Game.Combat;

namespace Game.Magic
{
    /// <summary>
    /// Единое место, где заклинания попадают по целям: снаряд, взрыв, хитскан и зоны
    /// строят магический DamagePacket и отдают его DamageResolver, а также
    /// уведомляют SpellReceiver. Не дублируем эту логику в каждом эффекте.
    /// </summary>
    public static class SpellHitResolver
    {
        private static readonly Collider[] OverlapBuffer = new Collider[64];
        private static readonly HashSet<DamageReceiver> HitReceivers = new HashSet<DamageReceiver>();
        private static readonly HashSet<SpellReceiver> HitSpellReceivers = new HashSet<SpellReceiver>();

        public static bool IsCaster(GameObject caster, Collider collider)
        {
            return caster != null && collider.transform.IsChildOf(caster.transform);
        }

        /// <summary>
        /// Попадание в одну цель. false — в коллайдере нет ни DamageReceiver, ни SpellReceiver
        /// (например, стена): ничего не произошло.
        /// </summary>
        public static bool HitCollider(SpellData spell, GameObject caster, Collider collider, int damage, bool canBeBlocked)
        {
            DamageReceiver receiver = collider.GetComponentInParent<DamageReceiver>();
            SpellReceiver spellReceiver = collider.GetComponentInParent<SpellReceiver>();
            if (receiver == null && spellReceiver == null) return false;

            if (receiver != null)
            {
                ApplyDamage(spell, caster, receiver, damage, canBeBlocked);
            }

            if (spellReceiver != null)
            {
                spellReceiver.Receive(spell, caster);
            }

            return true;
        }

        /// <summary>
        /// Урон по сферической области: каждая цель получает урон один раз за вызов.
        /// notifySpellReceivers = false нужен зонам, чтобы двери и т.п. не срабатывали на каждый тик.
        /// </summary>
        public static void HitArea(
    SpellData spell,
    GameObject caster,
    Vector3 center,
    float radius,
    int damage,
    bool canBeBlocked,
    bool affectCaster,
    LayerMask mask,
    bool notifySpellReceivers = true,
    bool ignoreDefense = false)
        {
            HitReceivers.Clear();
            HitSpellReceivers.Clear();

            int count = Physics.OverlapSphereNonAlloc(center, radius, OverlapBuffer, mask, QueryTriggerInteraction.Ignore);
            ApplyToOverlaps(count, spell, caster, damage, canBeBlocked, affectCaster, notifySpellReceivers, ignoreDefense);
        }

        /// <summary>
        /// Урон по области, заданной формой коллайдеров (Box/Sphere/Capsule; прочие — по их bounds).
        /// Цель, попавшая сразу в несколько коллайдеров, получает урон один раз за вызов.
        /// Сами коллайдеры-формы могут быть триггерами и не обязаны быть включены в физику
        /// как препятствие — запрос лишь читает их размер и положение.
        /// </summary>
        public static void HitShapes(
    SpellData spell,
    GameObject caster,
    Collider[] shapes,
    int damage,
    bool canBeBlocked,
    bool affectCaster,
    LayerMask mask,
    bool notifySpellReceivers = true,
    bool ignoreDefense = false)
        {
            HitReceivers.Clear();
            HitSpellReceivers.Clear();

            for (int i = 0; i < shapes.Length; i++)
            {
                Collider shape = shapes[i];
                if (shape == null || !shape.enabled) continue;

                int count = OverlapShape(shape, mask);
                ApplyToOverlaps(count, spell, caster, damage, canBeBlocked, affectCaster, notifySpellReceivers, ignoreDefense);
            }
        }

        /// <summary>Перебор найденного в OverlapBuffer. Множества HitReceivers/HitSpellReceivers очищает вызывающий метод.</summary>
        private static void ApplyToOverlaps(
    int count,
    SpellData spell,
    GameObject caster,
    int damage,
    bool canBeBlocked,
    bool affectCaster,
    bool notifySpellReceivers,
    bool ignoreDefense)
        {
            for (int i = 0; i < count; i++)
            {
                Collider collider = OverlapBuffer[i];
                if (!affectCaster && IsCaster(caster, collider)) continue;

                DamageReceiver receiver = collider.GetComponentInParent<DamageReceiver>();
                if (receiver != null && HitReceivers.Add(receiver))
                {
                    ApplyDamage(spell, caster, receiver, damage, canBeBlocked, ignoreDefense);
                }

                if (!notifySpellReceivers) continue;

                SpellReceiver spellReceiver = collider.GetComponentInParent<SpellReceiver>();
                if (spellReceiver != null && HitSpellReceivers.Add(spellReceiver))
                {
                    spellReceiver.Receive(spell, caster);
                }
            }
        }

        /// <summary>
        /// Overlap-запрос ровно по форме коллайдера с учётом его позиции, поворота и масштаба.
        /// Результат — в OverlapBuffer, возвращается количество найденных коллайдеров.
        /// </summary>
        private static int OverlapShape(Collider shape, LayerMask mask)
        {
            Transform t = shape.transform;
            Vector3 scale = t.lossyScale;
            float sx = Mathf.Abs(scale.x);
            float sy = Mathf.Abs(scale.y);
            float sz = Mathf.Abs(scale.z);

            switch (shape)
            {
                case SphereCollider sphere:
                    {
                        Vector3 center = t.TransformPoint(sphere.center);
                        float radius = sphere.radius * Mathf.Max(sx, sy, sz);
                        return Physics.OverlapSphereNonAlloc(center, radius, OverlapBuffer, mask, QueryTriggerInteraction.Ignore);
                    }

                case BoxCollider box:
                    {
                        Vector3 center = t.TransformPoint(box.center);
                        Vector3 halfExtents = new Vector3(box.size.x * sx, box.size.y * sy, box.size.z * sz) * 0.5f;
                        return Physics.OverlapBoxNonAlloc(center, halfExtents, OverlapBuffer, t.rotation, mask, QueryTriggerInteraction.Ignore);
                    }

                case CapsuleCollider capsule:
                    {
                        Vector3 axis;
                        float axisScale;
                        float radialScale;

                        switch (capsule.direction)
                        {
                            case 0: axis = t.right; axisScale = sx; radialScale = Mathf.Max(sy, sz); break;
                            case 1: axis = t.up; axisScale = sy; radialScale = Mathf.Max(sx, sz); break;
                            default: axis = t.forward; axisScale = sz; radialScale = Mathf.Max(sx, sy); break;
                        }

                        float radius = capsule.radius * radialScale;
                        float halfSegment = Mathf.Max(0f, capsule.height * axisScale * 0.5f - radius);
                        Vector3 center = t.TransformPoint(capsule.center);

                        return Physics.OverlapCapsuleNonAlloc(
                            center + axis * halfSegment, center - axis * halfSegment, radius,
                            OverlapBuffer, mask, QueryTriggerInteraction.Ignore);
                    }

                default:
                    {
                        // MeshCollider и прочие: приближение осевой коробкой (bounds). Для точной формы используйте Box/Sphere/Capsule.
                        Bounds bounds = shape.bounds;
                        return Physics.OverlapBoxNonAlloc(bounds.center, bounds.extents, OverlapBuffer, Quaternion.identity, mask, QueryTriggerInteraction.Ignore);
                    }
            }
        }

        /// <summary>
        /// Весь урон заклинания — стихийный (ElementalDamage со стихией spell.element;
        /// без стихии это нейтральная магия). Физической части у заклинаний нет.
        /// </summary>
        private static void ApplyDamage(SpellData spell, GameObject caster, DamageReceiver receiver, int damage, bool canBeBlocked, bool ignoreDefense = false)
        {
            var packet = new DamagePacket(
                source: caster,
                target: receiver.gameObject,
                basePhysicalDamage: 0,
                damageMultiplier: 1f,
                attackData: null,
                attackerStats: null,
                kind: DamageKind.Magic,
                canBeBlocked: canBeBlocked,
                element: spell != null ? spell.element : null,
                elementalDamage: damage,
                onHitEffects: spell != null ? spell.onHitEffects : null,
                ignoreDefense: ignoreDefense);

            DamageResolver.ResolveDamage(packet, receiver);
        }
    }
}
