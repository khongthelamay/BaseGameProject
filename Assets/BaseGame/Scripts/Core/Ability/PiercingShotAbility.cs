using System.Threading;
using BaseGame.Scripts.Enum;
using Core.SimplePool;
using Cysharp.Threading.Tasks;
using Manager;
using TW.ACacheEverything;
using UnityEngine;

namespace Core
{
    public partial class PiercingShotAbility : ActiveAbility
    {
        [field: SerializeField] public Projectile Projectile {get; private set;}
        [field: SerializeField] public Transform SpawnPosition {get; private set;}
        
        [field: SerializeField] public float ActiveRate {get; private set;} = 10f;
        [field: SerializeField] public float DamageScale {get; private set;} = 9;
        [field: SerializeField] public DamageType DamageType {get; private set;} = DamageType.Physical;
        
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
            await DelaySample(5, tickRate, ct);
            Owner.HeroAnim.PlayAttackAnimation(Owner.AttackSpeed);
            Projectile.Spawn(SpawnPosition.position, Quaternion.identity).Setup(Owner, EnemyTarget, DamageType)
                .WithComplete(OnProjectileMoveCompleteCache);
            await DelaySample(25, tickRate, ct);
        }
        [ACacheMethod]
        private void OnProjectileMoveComplete()
        {
            EnemyTarget.TakeDamage((int)(Owner.AttackDamage * DamageScale), DamageType);
        }
    }
}