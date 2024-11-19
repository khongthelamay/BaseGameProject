using System.Threading;
using Core.SimplePool;
using Cysharp.Threading.Tasks;
using TW.ACacheEverything;
using TW.Utility.CustomType;
using UnityEngine;

namespace Core
{
    public partial class RangeAttackAbility : ActiveAbility
    {
        [field: SerializeField] public DamageType DamageType { get; private set; } = DamageType.Physical;
        [field: SerializeField] public Projectile Projectile {get; private set;}
        [field: SerializeField] public Transform SpawnPosition {get; private set;}

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
            BigNumber damageDeal = Owner.AttackDamage;
            float attackSpeed = Owner.AttackSpeed;
            
            Owner.HeroAnim.PlayAttackAnimation(attackSpeed);
            Projectile.Spawn(SpawnPosition.position, Quaternion.identity)
                .Setup(Owner, EnemyTarget, damageDeal, DamageType)
                .WithComplete(OnProjectileMoveCompleteCache);
            await DelaySample(30, tickRate, ct);
        }
        [ACacheMethod("TW.Utility.CustomType")]
        private void OnProjectileMoveComplete(Hero ownerHero, Enemy targetEnemy, BigNumber damage, DamageType damageType)
        {
            targetEnemy.TakeDamage(damage, damageType);
        }
        
    }
}