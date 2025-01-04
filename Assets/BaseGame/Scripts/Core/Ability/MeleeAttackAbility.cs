using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.CustomType;


namespace Core
{
    public abstract class MeleeAttackAbility : NormalAttackAbility
    {
        public override async UniTask UseAbility(TickRate tickRate, CancellationToken ct)
        {
            BigNumber attackDamage = Owner.AttackDamage(out bool isCritical);
            float attackSpeed = Owner.AttackSpeed;
            Owner.SetFacingPosition(EnemyTarget.Transform.position);
            Owner.HeroAnim.PlayAttackAnimation(attackSpeed);
            await DelaySample(DelayFrame, tickRate, ct);
            if (!EnemyTarget.TakeDamage(EnemyTargetId, attackDamage, DamageType, isCritical)) return;
            OnAttackComplete();
            await DelaySample(30 - DelayFrame, tickRate, ct);
        }
    }
}