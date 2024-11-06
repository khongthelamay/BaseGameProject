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
        [field: SerializeField] public DamageType DamageType {get; private set;}

        private BattleManager BattleManagerCache { get; set; }
        public BattleManager BattleManager => BattleManagerCache ??= BattleManager.Instance;
        private Enemy EnemyTarget { get; set; }
        
        public MeleeAttackAbility(Hero owner, int levelUnlock, DamageType damageType) : base(owner, levelUnlock)
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
            EnemyTarget.TakeDamage(Owner.AttackDamage, DamageType);
            Owner.HeroAnim.PlayAttackAnimation(Owner.AttackSpeed);
            await DelaySample(30, tickRate, ct);

        }
        
    }
}