using UnityEngine;

namespace Game.Combat
{
    /// <summary>
    /// Правила статус-эффектов (stateless): выпал ли шанс наложения с учётом
    /// сопротивления цели и какой DamagePacket создаёт тик яда/ожога/кровотечения.
    /// Таймерами и списком активных эффектов занимается StatusEffectController.
    /// </summary>
    public static class StatusEffectResolver
    {
        /// <summary>Бросает шанс для каждого эффекта из списка и накладывает выпавшие.</summary>
        public static void ResolveOnHit(
            StatusEffectApplication[] effects,
            StatusEffectController controller,
            ResistanceProfileData profile)
        {
            if (effects == null || effects.Length == 0 || controller == null) return;

            foreach (StatusEffectApplication application in effects)
            {
                if (application == null || application.effect == null) continue;

                float resistancePercent = profile != null ? profile.GetStatusResistancePercent(application.effect) : 0f;
                float chance = Mathf.Clamp01(application.chancePercent / 100f * (1f - resistancePercent / 100f));

                if (Random.value < chance)
                {
                    controller.Apply(application.effect);
                }
                else
                {
                    Debug.Log($"StatusEffectResolver: {application.effect.name} не наложен (шанс {chance:P0})");
                }
            }
        }

        /// <summary>
        /// Один тик периодического урона. Идёт через обычный DamageResolver, поэтому
        /// сопротивления стихии и защита работают так же, как для любого другого урона.
        /// Тик не блокируется щитом и не парируется (AttackData = null).
        /// </summary>
        public static void ResolveTick(StatusEffectInstance instance, DamageReceiver receiver)
        {
            StatusEffectData data = instance.Data;

            int damage = data.tickDamage;
            if (data.tickMaxHealthPercent > 0f && receiver.Health != null)
            {
                // Минимум 1, чтобы процентный эффект не обнулялся на юнитах с малым HP.
                damage += Mathf.Max(1, Mathf.RoundToInt(receiver.Health.MaxHealth * data.tickMaxHealthPercent / 100f));
            }

            bool hasElement = data.tickElement != null;

            var packet = new DamagePacket(
                source: null,
                target: receiver.gameObject,
                basePhysicalDamage: hasElement ? 0 : damage,
                damageMultiplier: 1f,
                attackData: null,
                attackerStats: null,
                kind: DamageKind.Physical,
                canBeBlocked: false,
                element: data.tickElement,
                elementalDamage: hasElement ? damage : 0,
                ignoreDefense: data.tickIgnoresDefense);

            DamageResolver.ResolveDamage(packet, receiver);
        }
    }
}
