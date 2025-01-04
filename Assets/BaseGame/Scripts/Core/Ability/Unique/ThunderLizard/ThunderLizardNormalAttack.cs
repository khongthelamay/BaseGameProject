using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.CustomType;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "ThunderLizardNormalAttack", menuName = "Ability/ThunderLizard/ThunderLizardNormalAttack")]
    public class ThunderLizardNormalAttack : MeleeAttackAbility
    {
        private ThunderLizard OwnerThunderLizard { get; set; }
        public override Ability WithOwnerHero(Hero owner)
        {
            OwnerThunderLizard = owner as ThunderLizard;
            return base.WithOwnerHero(owner);
        }
        public override async UniTask UseAbility(TickRate tickRate, CancellationToken ct)
        {
            BigNumber attackDamage = Owner.AttackDamage(out bool isCritical);
            float attackSpeed = Owner.AttackSpeed;
            Owner.SetFacingPosition(EnemyTarget.Transform.position);
            Owner.HeroAnim.PlayAttackAnimation(attackSpeed);
            await DelaySample(DelayFrame, tickRate, ct);
            if (!EnemyTarget.TakeDamage(EnemyTargetId, attackDamage, DamageType, isCritical)) return;
            if (isCritical && OwnerThunderLizard.TryGetAbility(out StormSurgeAbility stormSurgeAbility))
            {
                stormSurgeAbility.IncreaseStackCount();
            }
            OnAttackComplete();
            await DelaySample(30 - DelayFrame, tickRate, ct);
        }

        public override void OnAttackComplete()
        {
            if (OwnerThunderLizard.TryGetAbility(out ElectricPrecisionAbility electricPrecisionAbility))
            {
                electricPrecisionAbility.AddAttackCount();
            }
            base.OnAttackComplete();
        }
    }
}