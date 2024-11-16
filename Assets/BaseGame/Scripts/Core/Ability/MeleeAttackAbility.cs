using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.CustomType;
using UnityEngine;

namespace Core
{
    public class MeleeAttackAbility : ActiveAbility
    {
        [field: SerializeField] private DamageType DamageType {get; set;}
        
        private Enemy EnemyTarget { get; set; }
        
        public MeleeAttackAbility(Hero owner, int levelUnlock,DamageType damageType) : base(owner, levelUnlock)
        {
            DamageType = damageType;
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
            
            Owner.HeroAnim.PlayAttackAnimation(attackSpeed);
            EnemyTarget.WillTakeDamage(damageDeal);
            // delay if needed
            EnemyTarget.TakeDamage(damageDeal, DamageType);
            await DelaySample(30, tickRate, ct);

        }
        
    }
}