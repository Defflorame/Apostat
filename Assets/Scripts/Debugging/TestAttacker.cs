using UnityEngine;
using UnityEngine.InputSystem;
using Game.Combat;
using Game.Items;

namespace Game.Debugging
{
    /// <summary>
    /// ВРЕМЕННЫЙ инструмент. Не часть боевой архитектуры — удалите этот
    /// файл, когда появятся реальные атаки врагов (Phase 8).
    ///
    /// По нажатию клавиши T шлёт DamagePacket в указанный DamageReceiver,
    /// как будто это атака противника — нужен, чтобы проверить Block/Parry
    /// прямо сейчас, не дожидаясь Enemy AI.
    /// </summary>
    public class TestAttacker : MonoBehaviour
    {
        [SerializeField] private DamageReceiver target;
        [SerializeField] private AttackData simulatedAttack;
        [Tooltip("Необязательно. Оружие, чьи стихия и эффекты при попадании добавляются к симулируемой атаке.")]
        [SerializeField] private WeaponData simulatedWeapon;

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.tKey.wasPressedThisFrame)
            {
                SimulateAttack();
            }
        }

        private void SimulateAttack()
        {
            if (target == null || simulatedAttack == null)
            {
                Debug.LogWarning("TestAttacker: target или simulatedAttack не назначены.", this);
                return;
            }

            Debug.Log($"TestAttacker: simulating attack ({simulatedAttack.name}) -> {target.name}");

            var packet = new DamagePacket(
                source: gameObject,
                target: target.gameObject,
                basePhysicalDamage: simulatedAttack.damage,
                damageMultiplier: 1f,
                attackData: simulatedAttack,
                element: simulatedWeapon != null ? simulatedWeapon.element : null,
                elementalDamage: simulatedWeapon != null ? simulatedWeapon.GetElementalDamage(simulatedAttack) : 0,
                onHitEffects: simulatedWeapon != null ? simulatedWeapon.onHitEffects : null
            );

            DamageResolver.ResolveDamage(packet, target);
        }
    }
}
