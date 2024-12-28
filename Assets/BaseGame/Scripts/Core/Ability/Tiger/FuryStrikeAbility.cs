using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.CustomType;
using UnityEngine;

namespace Core.TigerAbility
{
    [CreateAssetMenu(fileName = "FuryStrikeAbility", menuName = "Ability/Tiger/FuryStrikeAbility")]
    public class FuryStrikeAbility : ActiveAbility, IAbilityTargetAnEnemy 
    {
        [field: SerializeField] public int FuryStrikeRate {get; private set;}
        [field: SerializeField] public DamageType DamageType {get; private set;}
        [field: SerializeField] public int DelayFrame {get; private set;}
        public Enemy EnemyTarget { get; set; }
        public int EnemyTargetId { get; set; }
        public Tiger OwnerTiger { get; set; }

        public override Ability WithOwnerHero(Hero owner)
        {
            OwnerTiger = owner as Tiger;
            return base.WithOwnerHero(owner);
        }
        public override bool CanUseAbility()
        {
            if (Random.Range(0, 100) >= FuryStrikeRate) return false;
            return this.IsFindAnyEnemyTarget();
        }

        public override async UniTask UseAbility(TickRate tickRate, CancellationToken ct)
        {
            BigNumber attackDamage = Owner.AttackDamage(out bool isCritical);
            float attackSpeed = Owner.AttackSpeed;
            
            Owner.SetFacingPosition(EnemyTarget.Transform.position);
            Owner.HeroAnim.PlayAttackAnimation(attackSpeed);
            await DelaySample(DelayFrame, tickRate, ct);
            if (!EnemyTarget.TakeDamage(EnemyTargetId, attackDamage, DamageType, isCritical)) return;
            OwnerTiger.ChangeFuryPoint(1);
            await DelaySample(30 - DelayFrame, tickRate, ct);
        }
        
        public override void OnEnterBattleField()
        {

        }

        public override void OnExitBattleField()
        {

        }
        public void ChangeFuryStrikeRate(int value)
        {
            FuryStrikeRate += value;
        }
    }
}