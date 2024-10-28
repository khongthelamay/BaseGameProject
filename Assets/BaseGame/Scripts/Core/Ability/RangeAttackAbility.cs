using System.Threading;
using BaseGame.Scripts.Enum;
using Core.SimplePool;
using Cysharp.Threading.Tasks;
using Manager;
using TW.ACacheEverything;
using UnityEngine;

namespace Core
{
    public partial class RangeAttackAbility : ActiveAbility
    {
        [field: SerializeField] public DamageType DamageType { get; private set; } = DamageType.Physical;
        [field: SerializeField] public Projectile Projectile {get; private set;}
        [field: SerializeField] public Transform SpawnPosition {get; private set;}
        private BattleManager BattleManagerCache { get; set; }
        public BattleManager BattleManager => BattleManagerCache ??= BattleManager.Instance;
        private Enemy EnemyTarget { get; set; }
    
        
        public RangeAttackAbility(Hero owner, int levelUnlock, Projectile projectile, Transform spawnPosition) : base(owner, levelUnlock)
        {
            Projectile = projectile;
            SpawnPosition = spawnPosition;
        }

        public override void OnEnterBattleField()
        {
            
        }

        public override void OnExitBattleField()
        {
            
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
            Projectile.Spawn(SpawnPosition.position, Quaternion.identity).Setup(Owner, EnemyTarget, DamageType)
                .WithComplete(OnProjectileMoveCompleteCache);
            await DelaySample(30, tickRate, ct);
        }
        [ACacheMethod]
        private void OnProjectileMoveComplete()
        {
            EnemyTarget.TakeDamage(Owner.AttackDamage, DamageType);
        }
        
    }
}