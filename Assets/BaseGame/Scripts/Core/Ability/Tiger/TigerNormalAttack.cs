using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.CustomType;
using UnityEngine;

namespace Core
{
    public class TigerNormalAttack : ActiveAbility
    {
        [field: SerializeField] private DamageType DamageType {get; set;} = DamageType.Physical;
        [field: SerializeField] private int FuryGainRate {get; set;} = 5;

        private Enemy EnemyTarget { get; set; }
        private Tiger OwnerTiger { get; set; }
        public TigerNormalAttack(Hero owner, int levelUnlock) : base(owner, levelUnlock)
        {
            OwnerTiger = (Tiger) owner;
        }
        public override bool CanUseAbility()
        {
            if (!BattleManager.TryGetEnemyInAttackRange(Owner, out Enemy target)) return false;
            EnemyTarget = target;
            return true;
        }

        public override void OnEnterBattleField()
        {
            
        }

        public override void OnExitBattleField()
        {
            
        }


        public override async UniTask UseAbility(TickRate tickRate, CancellationToken ct)
        {
            BigNumber damageDeal = Owner.AttackDamage;
            float attackSpeed = Owner.AttackSpeed;
            
            EnemyTarget.WillTakeDamage(damageDeal);
            Owner.SetFacingPosition(EnemyTarget.Transform.position);
            Owner.HeroAnim.PlayAttackAnimation(attackSpeed);
            await DelaySample(8, tickRate, ct);

            EnemyTarget.TakeDamage(damageDeal, DamageType);
            if (Random.Range(0, 100) < FuryGainRate + OwnerTiger.BonusFuryRate)
            {
                OwnerTiger.AddFuryPoint(1);
            }
            await DelaySample(22, tickRate, ct);

        }
        
    }
}