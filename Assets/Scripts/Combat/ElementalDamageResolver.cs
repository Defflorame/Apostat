using UnityEngine;

namespace Game.Combat
{
    /// <summary>
    /// Считает стихийную часть урона пакета: ElementalDamage × множитель заряда
    /// × (1 − magicResistance, только для DamageKind.Magic) × множитель стихии
    /// из профиля цели. Защиту (Defense) НЕ применяет — это делает DamageResolver
    /// один раз на сумму физической и стихийной частей. Возвращает float, округляет
    /// DamageResolver в самом конце.
    /// </summary>
    public static class ElementalDamageResolver
    {
        public static float Calculate(DamagePacket packet, DamageReceiver receiver)
        {
            if (packet.ElementalDamage <= 0) return 0f;

            float damage = packet.ElementalDamage * packet.DamageMultiplier;

            ResistanceProfileData profile = receiver.Resistance;
            if (packet.Kind == DamageKind.Magic && profile != null)
            {
                damage *= 1f - profile.GetMagicResistancePercent() / 100f;
            }

            if (packet.Element != null)
            {
                float percent = receiver.GetElementPercent(packet.Element);
                damage *= percent / 100f;

                if (!Mathf.Approximately(percent, 100f))
                {
                    Debug.Log($"ElementalDamageResolver: {packet.Element.name} против {receiver.name} — {percent:F0}% урона");
                }
            }

            return damage;
        }
    }
}
