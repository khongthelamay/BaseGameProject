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

        public override async UniTask UseAbility(TickRate tickRate, CancellationToken ct)
        {
            EnemyTarget.TakeDamage(Owner.AttackDamage, DamageType);
            Owner.HeroAnim.PlayAttackAnimation(Owner.AttackSpeed);
            int timeDelay = (int)(1000 / tickRate.ToValue() / Owner.AttackSpeed);
            await UniTask.Delay(timeDelay, cancellationToken: ct);
        }

        public override Ability CreateAbility(Hero owner)
        {
            return new MeleeAttackAbility(owner, LevelUnlock, DamageType);
        }
    }
}