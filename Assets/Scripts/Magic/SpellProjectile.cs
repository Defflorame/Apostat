using UnityEngine;
using Game.Combat;

namespace Game.Magic
{
    /// <summary>
    /// Снаряд заклинания. Урон считает DamageResolver, а не снаряд.
    /// Префаб: коллайдер + Rigidbody + этот скрипт (isTrigger/kinematic выставляются в Awake).
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class SpellProjectile : MonoBehaviour
    {
        private SpellData _spell;
        private GameObject _caster;
        private int _damage;
        private bool _hasHit;

        private void Awake()
        {
            Rigidbody body = GetComponent<Rigidbody>();
            body.isKinematic = true;
            body.useGravity = false;
            GetComponent<Collider>().isTrigger = true;
        }

        public void Initialize(SpellData spell, GameObject caster, int damage)
        {
            _spell = spell;
            _caster = caster;
            _damage = damage;
            Destroy(gameObject, spell.projectileLifetime);
        }

        private void Update()
        {
            if (_spell == null) return;
            transform.position += transform.forward * (_spell.projectileSpeed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_hasHit || _spell == null) return;
            if (_caster != null && other.transform.IsChildOf(_caster.transform)) return;

            DamageReceiver receiver = other.GetComponentInParent<DamageReceiver>();
            SpellReceiver spellReceiver = other.GetComponentInParent<SpellReceiver>();

            if (receiver == null && spellReceiver == null)
            {
                if (other.isTrigger) return; // чужие триггеры (хитбоксы и т.п.) игнорируем
                Destroy(gameObject);          // стена/пол
                return;
            }

            _hasHit = true;

            if (receiver != null)
            {
                var packet = new DamagePacket(
                    source: _caster,
                    target: receiver.gameObject,
                    basePhysicalDamage: _damage,
                    damageMultiplier: 1f,
                    attackData: null);

                DamageResolver.ResolveDamage(packet, receiver);
            }

            if (spellReceiver != null)
            {
                spellReceiver.Receive(_spell, _caster);
            }

            Destroy(gameObject);
        }
    }
}