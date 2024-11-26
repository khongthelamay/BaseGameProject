using System.Threading;
using Core.SimplePool;
using Cysharp.Threading.Tasks;
using LitMotion;
using TW.ACacheEverything;
using TW.Utility.CustomType;
using UnityEngine;

namespace Core
{
    // public partial class ArrowStormAbility : ActiveCooldownAbility
    // {
    //
    //     [field: SerializeField] private float DamageScale { get; set; } = 10f;
    //     [field: SerializeField] private DamageType DamageType { get; set; } = DamageType.Physical;
    //     [field: SerializeField] private Projectile Projectile { get; set; }
    //     [field: SerializeField] private Transform SpawnPosition { get; set; }
    //     private Enemy EnemyTarget { get; set; }
    //
    //     public ArrowStormAbility(Archer owner, float cooldown, Projectile projectile, Transform spawnPosition) : base(
    //         owner, cooldown)
    //     {
    //         Projectile = projectile;
    //         SpawnPosition = spawnPosition;
    //     }
    //
    //     public override void OnEnterBattleField()
    //     {
    //         StartCooldownHandle();
    //     }
    //
    //     public override void OnExitBattleField()
    //     {
    //         StopCooldownHandle();
    //     }
    //
    //     public override bool CanUseAbility()
    //     {
    //         if (IsOnCooldown) return false;
    //         if (!BattleManager.TryGetEnemyInAttackRange(Owner, out Enemy target)) return false;
    //         EnemyTarget = target;
    //         return true;
    //     }
    //
    //
    //     public override async UniTask UseAbility(TickRate tickRate, CancellationToken ct)
    //     {
    //         CooldownTimer = Cooldown;
    //         BigNumber damageDeal = Owner.AttackDamage * DamageScale;
    //         float attackSpeed = Owner.AttackSpeed;
    //         
    //         Owner.HeroAnim.PlaySkillAnimation(attackSpeed);
    //         await DelaySample(8, tickRate, ct);
    //         Projectile.Spawn(SpawnPosition.position, Quaternion.identity)
    //             .Setup(Owner, EnemyTarget, damageDeal, DamageType)
    //             .WithComplete(OnProjectileMoveCompleteCache);
    //         await DelaySample(8, tickRate, ct);
    //         
    //         Projectile.Spawn(SpawnPosition.position, Quaternion.identity)
    //             .Setup(Owner, EnemyTarget, damageDeal, DamageType)
    //             .WithComplete(OnProjectileMoveCompleteCache);
    //         await DelaySample(8, tickRate, ct);
    //         
    //         Projectile.Spawn(SpawnPosition.position, Quaternion.identity)
    //             .Setup(Owner, EnemyTarget, damageDeal, DamageType)
    //             .WithComplete(OnProjectileMoveCompleteCache);
    //         await DelaySample(6, tickRate, ct);
    //         
    //         ResetCooldown();
    //     }
    //
    //     [ACacheMethod("TW.Utility.CustomType")]
    //     private void OnProjectileMoveComplete(Hero ownerHero, Enemy targetEnemy, BigNumber damage, DamageType damageType)
    //     {
    //         targetEnemy.TakeDamage(damage, damageType);
    //     }
    //     
    //
    //     [ACacheMethod]
    //     private void OnCooldownHandleUpdate(float time)
    //     {
    //         CooldownTimer = time;
    //     }
    //
    //     [ACacheMethod]
    //     private void OnCooldownHandleComplete()
    //     {
    //         IsOnCooldown = false;
    //     }
    // }
}