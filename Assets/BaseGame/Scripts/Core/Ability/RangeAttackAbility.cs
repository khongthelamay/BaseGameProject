using System.Threading;
using BaseGame.Scripts.Enum;
using Core.SimplePool;
using Cysharp.Threading.Tasks;
using Manager;
using UnityEngine;

namespace Core
{
    public class RangeAttackAbility : ActiveAbility
    {
        [field: SerializeField] public DamageType DamageType {get; private set;}
        [field: SerializeField] public Projectile Projectile {get; private set;}
        [field: SerializeField] private Vector3 SpawnOffset {get; set;}
        private BattleManager BattleManagerCache { get; set; }
        public BattleManager BattleManager => BattleManagerCache ??= BattleManager.Instance;
        private Enemy EnemyTarget { get; set; }
    
        

        public RangeAttackAbility(Hero owner, int levelUnlock, DamageType damageType, Projectile projectile, Vector3 spawnOffset) : base(owner, levelUnlock)
        {
            DamageType = damageType;
            Projectile = projectile;
            SpawnOffset = spawnOffset;
        }
        public override bool CanUseAbility()
        {
            if (!BattleManager.TryGetEnemyInAttackRange(Owner, out Enemy target)) return false;
            EnemyTarget = target;
            return true;
        }

        public override async UniTask UseAbility(TickRate tickRate, CancellationToken ct)
        {
            Owner.HeroAnim.PlayAttackAnimation(Owner.AttackSpeed);
            Projectile.Spawn(Owner.Transform.position + SpawnOffset, Quaternion.identity).Setup(Owner, EnemyTarget, DamageType);
            int timeDelay = (int)(1000 / tickRate.ToValue() / Owner.AttackSpeed);
            await UniTask.Delay(timeDelay, cancellationToken: ct);
        }

        public override Ability CreateAbility(Hero owner)
        {
            return new RangeAttackAbility(owner, LevelUnlock, DamageType, Projectile, SpawnOffset);
        }
    }
}