using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.CustomType;


namespace Core
{
    public abstract class MeleeAttackAbility : NormalAttackAbility
    {
        public override async UniTask UseAbility(TickRate tickRate, CancellationToken ct)
        {
            BigNumber damageDeal = Owner.AttackDamage;
            float attackSpeed = Owner.AttackSpeed;
            
            EnemyTarget.WillTakeDamage(damageDeal);
            Owner.SetFacingPosition(EnemyTarget.Transform.position);
            Owner.HeroAnim.PlayAttackAnimation(attackSpeed);
            await DelaySample(DelayFrame, tickRate, ct);
            EnemyTarget.TakeDamage(damageDeal, DamageType);
            OnAttackComplete();
            await DelaySample(30 - DelayFrame, tickRate, ct);
        }
    }
}