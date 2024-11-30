using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.CustomType;
using UnityEngine;

namespace Core.WolfAbility
{
    [CreateAssetMenu(fileName = "FeralStrikeAbility", menuName = "Ability/Wolf/FeralStrikeAbility")]
    public class FeralStrikeAbility : ActiveAbility, IAbilityTargetAnEnemy
    {
        [field: SerializeField] public int FeralStrikeRate {get; private set;}
        [field: SerializeField] public DamageType DamageType {get; private set;}
        [field: SerializeField] public int DelayFrame {get; private set;}
        [field: SerializeField] public float DamageScale {get; private set;}
        public Enemy EnemyTarget { get; set; }
        public int EnemyTargetId { get; set; }

        public override bool CanUseAbility()
        {
            if (Random.Range(0, 100) >= FeralStrikeRate) return false;
            return this.IsFindAnyEnemyTarget();
        }

        public override async UniTask UseAbility(TickRate tickRate, CancellationToken ct)
        {
            BigNumber damageDeal = Owner.AttackDamage(out bool isCritical) * DamageScale;
            float attackSpeed = Owner.AttackSpeed;
            
            if (!EnemyTarget.WillTakeDamage(EnemyTargetId, damageDeal)) return;
            Owner.SetFacingPosition(EnemyTarget.Transform.position);
            Owner.HeroAnim.PlayAttackAnimation(attackSpeed);
            await DelaySample(DelayFrame, tickRate, ct);
            if (!EnemyTarget.TakeDamage(EnemyTargetId, damageDeal, DamageType, isCritical)) return;
            await DelaySample(30 - DelayFrame, tickRate, ct);
        }
        
        public override void OnEnterBattleField()
        {

        }

        public override void OnExitBattleField()
        {

        }


    }
}