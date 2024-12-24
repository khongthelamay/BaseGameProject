using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.CustomType;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "RendingClawsAbility", menuName = "Ability/Bear/RendingClawsAbility")]
    public class RendingClawsAbility : ActiveAbility, IAbilityTargetAnEnemy
    {
        [field: SerializeField] public int AttackCount {get; private set;}
        [field: SerializeField] public DamageType DamageType {get; private set;}
        [field: SerializeField] public int DelayFrame {get; private set;}
        
        public Enemy EnemyTarget { get; set; }
        public int EnemyTargetId { get; set; }
        public BigNumber FinalDamage { get; set; }
        public Bear OwnerBear { get; private set; }
        public override Ability WithOwnerHero(Hero owner)
        {
            OwnerBear = owner as Bear;
            return base.WithOwnerHero(owner);
        }

        public override void OnEnterBattleField()
        {
        }

        public override void OnExitBattleField()
        {
        }

        public override bool CanUseAbility()
        {
            if (AttackCount < 3) return false;
            return this.IsFindAnyEnemyTarget();
        }

        public override async UniTask UseAbility(TickRate tickRate, CancellationToken ct)
        {
            BigNumber attackDamage = Owner.CriticalAttackDamage();
            float attackSpeed = Owner.AttackSpeed;
            if (!EnemyTarget.WillTakeDamage(EnemyTargetId, attackDamage, DamageType, out BigNumber finalDamage)) return;
            FinalDamage = finalDamage;
            Owner.SetFacingPosition(EnemyTarget.Transform.position);
            Owner.HeroAnim.PlayAttackAnimation(attackSpeed);
            await DelaySample(DelayFrame, tickRate, ct);
            if (!EnemyTarget.TakeDamage(EnemyTargetId, FinalDamage, DamageType, true)) return;
            await DelaySample(30 - DelayFrame, tickRate, ct);
            AttackCount -= 3;
        }
        public void AddAttackCount()
        {
            AttackCount++;
        }


    }
}