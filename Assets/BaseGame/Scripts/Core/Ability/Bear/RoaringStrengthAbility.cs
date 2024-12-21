using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.CustomType;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "RoaringStrengthAbility", menuName = "Ability/Bear/RoaringStrengthAbility")]
    public class RoaringStrengthAbility : ActiveAbility, IAbilityTargetAnEnemy
    {
        [field: SerializeField] public int RoaringStrengthRate {get; private set;}
        [field: SerializeField] public DamageType DamageType {get; private set;}
        [field: SerializeField] public int DamageDelayFrame {get; private set;}
        [field: SerializeField] public float DamageScale {get; private set;}
        public Enemy EnemyTarget { get; set; }
        public int EnemyTargetId { get; set; }
        public override void OnEnterBattleField()
        {
        }

        public override void OnExitBattleField()
        {
        }

        public override bool CanUseAbility()
        {
            if (Random.Range(0, 100) >= RoaringStrengthRate) return false;
            return this.IsFindAnyEnemyTarget();
        }

        public override async UniTask UseAbility(TickRate tickRate, CancellationToken ct)
        {
            BigNumber damageDeal = Owner.AttackDamage(out bool isCritical) * DamageScale;
            float attackSpeed = Owner.AttackSpeed;
            
            if (!EnemyTarget.WillTakeDamage(EnemyTargetId, damageDeal)) return;
            Owner.SetFacingPosition(EnemyTarget.Transform.position);
            Owner.HeroAnim.PlaySkill1Animation(attackSpeed);
            await DelaySample(DamageDelayFrame, tickRate, ct);
            if (!EnemyTarget.TakeDamage(EnemyTargetId, damageDeal, DamageType, isCritical)) return;
            await DelaySample(30 - DamageDelayFrame, tickRate, ct);
        }


    }
}