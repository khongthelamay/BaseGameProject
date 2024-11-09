using System.Threading;
using BaseGame.Scripts.Enum;
using Core.SimplePool;
using Cysharp.Threading.Tasks;
using LitMotion;
using Manager;
using TW.ACacheEverything;
using UnityEngine;

namespace Core
{
    public partial class ArrowStormAbility : ActiveAbility
    {
        [field: SerializeField] public Projectile Projectile { get; private set; }
        [field: SerializeField] public Transform SpawnPosition { get; private set; }

        [field: SerializeField] public float Cooldown { get; private set; } = 3f;
        [field: SerializeField] public float DamageScale { get; private set; } = 10f;
        [field: SerializeField] public DamageType DamageType { get; private set; } = DamageType.Physical;
        [field: SerializeField] private bool IsOnCooldown { get; set; }

        private Enemy EnemyTarget { get; set; }
        [field: SerializeField] private float CooldownTimer { get; set; }
        private MotionHandle CooldownHandle { get; set; }

        public ArrowStormAbility(Archer owner, int levelUnlock, Projectile projectile, Transform spawnPosition) : base(
            owner, levelUnlock)
        {
            Projectile = projectile;
            SpawnPosition = spawnPosition;
            CooldownTimer = Cooldown;
            IsOnCooldown = true;
        }

        public override void OnEnterBattleField()
        {
            StartCooldownHandle();
        }

        public override void OnExitBattleField()
        {
            StopCooldownHandle();
        }

        public override bool CanUseAbility()
        {
            if (IsOnCooldown) return false;
            if (!BattleManager.TryGetEnemyInAttackRange(Owner, out Enemy target)) return false;
            EnemyTarget = target;
            return true;
        }


        public override async UniTask UseAbility(TickRate tickRate, CancellationToken ct)
        {
            CooldownTimer = Cooldown;
            Owner.HeroAnim.PlaySkillAnimation(Owner.AttackSpeed);
            await DelaySample(8, tickRate, ct);
            Projectile.Spawn(SpawnPosition.position, Quaternion.identity).Setup(Owner, EnemyTarget, DamageType)
                .WithComplete(OnProjectileMoveCompleteCache);
            await DelaySample(8, tickRate, ct);
            
            Projectile.Spawn(SpawnPosition.position, Quaternion.identity).Setup(Owner, EnemyTarget, DamageType)
                .WithComplete(OnProjectileMoveCompleteCache);
            await DelaySample(8, tickRate, ct);
            
            Projectile.Spawn(SpawnPosition.position, Quaternion.identity).Setup(Owner, EnemyTarget, DamageType)
                .WithComplete(OnProjectileMoveCompleteCache);
            await DelaySample(6, tickRate, ct);
            
            StartCooldownHandle();
        }

        [ACacheMethod]
        private void OnProjectileMoveComplete()
        {
            EnemyTarget.TakeDamage((int)(Owner.AttackDamage * DamageScale), DamageType);
        }

        private void StartCooldownHandle()
        {
            IsOnCooldown = true;
            CooldownHandle = LMotion.Create(CooldownTimer, 0, CooldownTimer)
                .WithOnComplete(OnCooldownHandleCompleteCache)
                .Bind(OnCooldownHandleUpdateCache);
        }

        private void StopCooldownHandle()
        {
            CooldownHandle.TryCancel();
        }

        [ACacheMethod]
        private void OnCooldownHandleUpdate(float time)
        {
            CooldownTimer = time;
        }

        [ACacheMethod]
        private void OnCooldownHandleComplete()
        {
            IsOnCooldown = false;
        }
    }
}