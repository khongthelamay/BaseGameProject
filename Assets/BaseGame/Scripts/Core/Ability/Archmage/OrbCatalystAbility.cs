using System.Threading;
using BaseGame.Scripts.Enum;
using Core.SimplePool;
using Cysharp.Threading.Tasks;
using TW.ACacheEverything;
using UnityEngine;

namespace Core
{
    public partial class OrbCatalystAbility : ActiveAbility
    {
        [field: SerializeField] public DamageType DamageType { get; private set; } = DamageType.Magical;
        [field: SerializeField] public Projectile Projectile {get; private set;}
        [field: SerializeField] public Transform SpawnPosition {get; private set;}

        private Enemy EnemyTarget { get; set; }
        private Archmage OwnerArchmage { get; set; }
        
        public OrbCatalystAbility(Hero owner, int levelUnlock, Projectile projectile, Transform spawnPosition) : base(owner, levelUnlock)
        {
            Projectile = projectile;
            SpawnPosition = spawnPosition;
            OwnerArchmage = (Archmage) owner;
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
            await DelaySample(8, tickRate, ct);
            Projectile.Spawn(SpawnPosition.position, Quaternion.identity).Setup(Owner, EnemyTarget, DamageType)
                .WithComplete(OnProjectileMoveCompleteCache);
            await DelaySample(22, tickRate, ct);
            OwnerArchmage.SpawnRandomOrb();
            
        }
        [ACacheMethod]
        private void OnProjectileMoveComplete()
        {
            EnemyTarget.TakeDamage(Owner.AttackDamage, DamageType);
        }
    }
}