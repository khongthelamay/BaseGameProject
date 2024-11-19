using System.Threading;
using Core.SimplePool;
using Cysharp.Threading.Tasks;
using TW.ACacheEverything;
using TW.Utility.CustomType;
using UnityEngine;

namespace Core
{
    public partial class QueenNormalAttack : ActiveAbility
    {
        [field: SerializeField] private DamageType DamageType { get; set; } = DamageType.Physical;
        [field: SerializeField] private Projectile Projectile {get; set;}
        [field: SerializeField] private Transform SpawnPosition {get; set;}
        private Enemy EnemyTarget { get; set; }
        private Queen OwnerQueen { get; set; }
        public QueenNormalAttack(Hero owner, int levelUnlock, Projectile projectile, Transform spawnPosition) : base(owner, levelUnlock)
        {
            Projectile = projectile;
            SpawnPosition = spawnPosition;
            OwnerQueen = (Queen) owner;
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
            
            EnemyTarget.WillTakeDamage(damageDeal);
            Owner.SetFacingPosition(EnemyTarget.Transform.position);
            Owner.HeroAnim.PlayAttackAnimation(attackSpeed);
            await DelaySample(6, tickRate, ct);
            Projectile.Spawn(SpawnPosition.position, Quaternion.identity).Setup(Owner, EnemyTarget, damageDeal, DamageType)
                .WithComplete(OnProjectileMoveCompleteCache);
            if (Random.Range(0, 100) < OwnerQueen.CooldownSurgeRate)
            {
                OwnerQueen.ReduceCooldownAllHeroAround();
            }
            await DelaySample(24, tickRate, ct);
        }
        [ACacheMethod("TW.Utility.CustomType")]
        private void OnProjectileMoveComplete(Hero ownerHero, Enemy targetEnemy, BigNumber damage, DamageType damageType)
        {
            targetEnemy.TakeDamage(damage, damageType);
        }
    }
}