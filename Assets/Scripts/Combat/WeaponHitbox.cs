using UnityEngine;

namespace Game.Combat
{
    /// <summary>
    /// Отвечает только за обнаружение попадания оружия по цели.
    /// НЕ рассчитывает урон — это задача DamageResolver.
    /// Цепочка: WeaponHitbox → Target detected → DamagePacket → DamageResolver → DamageReceiver.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class WeaponHitbox : MonoBehaviour
    {
        private Collider _collider;
        private GameObject _owner;
        private AttackRuntime _currentAttack;
        private bool _isActive;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            _collider.isTrigger = true;
            _collider.enabled = false;
        }

        /// <summary>Владелец оружия (источник урона) — игрок или враг.</summary>
        public void SetOwner(GameObject owner)
        {
            _owner = owner;
        }

        /// <summary>
        /// Растягивает коллайдер вдоль локальной оси Z (forward оружия) до длины range.
        /// Предполагается, что pivot хитбокса стоит у рукояти/начала клинка.
        /// </summary>
        public void ApplyRange(float range)
        {
            switch (_collider)
            {
                case BoxCollider box:
                    Vector3 size = box.size;
                    size.z = Mathf.Max(0.01f, range);
                    box.size = size;

                    Vector3 center = box.center;
                    center.z = size.z * 0.5f;
                    box.center = center;
                    break;

                case CapsuleCollider capsule:
                    capsule.height = Mathf.Max(capsule.radius * 2f, range);

                    Vector3 capsuleCenter = capsule.center;
                    capsuleCenter.z = capsule.height * 0.5f;
                    capsule.center = capsuleCenter;
                    break;

                default:
                    Debug.LogWarning($"WeaponHitbox: ApplyRange поддерживает только BoxCollider/CapsuleCollider, у {name} другой тип коллайдера.", this);
                    break;
            }
        }

        public void Activate(AttackRuntime attackRuntime)
        {
            _currentAttack = attackRuntime;
            _isActive = true;
            _collider.enabled = true;
        }

        public void Deactivate()
        {
            _isActive = false;
            _collider.enabled = false;
            _currentAttack = null;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_isActive || _currentAttack == null) return;

            DamageReceiver receiver = other.GetComponentInParent<DamageReceiver>();
            if (receiver == null) return;
            if (_currentAttack.AlreadyHitTargets.Contains(receiver)) return;

            _currentAttack.AlreadyHitTargets.Add(receiver);

            var packet = new DamagePacket(
            source: _owner,
            target: receiver.gameObject,
            basePhysicalDamage: _currentAttack.AttackData.damage,
            damageMultiplier: _currentAttack.DamageMultiplier,
            attackData: _currentAttack.AttackData);

            DamageResolver.ResolveDamage(packet, receiver);
        }
    }
}