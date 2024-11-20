using System.Threading;
using Core.SimplePool;
using Cysharp.Threading.Tasks;
using TW.ACacheEverything;
using TW.Utility.CustomType;
using UnityEngine;

namespace Core
{
    public partial class PiercingShotAbility : ActiveAbility
    {
        
        [field: SerializeField] private float ActiveRate {get; set;} = 10f;
        [field: SerializeField] private float DamageScale {get; set;} = 9;
        [field: SerializeField] private DamageType DamageType {get; set;} = DamageType.Physical;
        [field: SerializeField] private Transform SpawnPosition {get; set;}
        [field: SerializeField] private Projectile Projectile {get; set;}
        
        private Enemy EnemyTarget { get; set; }

        public PiercingShotAbility(Archer owner, int levelUnlock, Projectile projectile, Transform spawnPosition) : base(owner, levelUnlock)
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
            float rate = Random.Range(0, 100);
            if (rate > ActiveRate) return false;
            if (!BattleManager.TryGetEnemyInAttackRange(Owner, out Enemy target)) return false;
            EnemyTarget = target;
            return true;
        }
        

        public override async UniTask UseAbility(TickRate tickRate, CancellationToken ct)
        {
            BigNumber damageDeal = Owner.AttackDamage * DamageScale;
            float attackSpeed = Owner.AttackSpeed;
            
            await DelaySample(5, tickRate, ct);
            Owner.HeroAnim.PlayAttackAnimation(attackSpeed);
            Projectile.Spawn(SpawnPosition.position, Quaternion.identity)
                .Setup(Owner, EnemyTarget, damageDeal, DamageType)
                .WithComplete(OnProjectileMoveCompleteCache);
            await DelaySample(25, tickRate, ct);
        }
        [ACacheMethod("TW.Utility.CustomType")]
        private void OnProjectileMoveComplete(Hero ownerHero, Enemy targetEnemy, BigNumber damage, DamageType damageType)
        {
            targetEnemy.TakeDamage(damage, damageType);
        }
    }
}