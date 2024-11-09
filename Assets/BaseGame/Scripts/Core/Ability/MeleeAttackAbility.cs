using System;
using System.Threading;
using BaseGame.Scripts.Enum;
using Cysharp.Threading.Tasks;
using Manager;
using UnityEngine;

namespace Core
{
    public class MeleeAttackAbility : ActiveAbility
    {
        [field: SerializeField] private int SampleDelay {get; set;}
        [field: SerializeField] private DamageType DamageType {get; set;}
        
        private Enemy EnemyTarget { get; set; }
        
        public MeleeAttackAbility(Hero owner, int levelUnlock, int sampleDelay,DamageType damageType) : base(owner, levelUnlock)
        {
            SampleDelay = sampleDelay;
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
            Owner.HeroAnim.PlayAttackAnimation(Owner.AttackSpeed);
            await DelaySample(SampleDelay, tickRate, ct);
            EnemyTarget.TakeDamage(Owner.AttackDamage, DamageType);
            await DelaySample(30 - SampleDelay, tickRate, ct);

        }
        
    }
}