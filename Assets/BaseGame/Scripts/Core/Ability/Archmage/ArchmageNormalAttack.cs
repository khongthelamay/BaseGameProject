﻿using System.Threading;
using Core.SimplePool;
using Cysharp.Threading.Tasks;
using TW.ACacheEverything;
using TW.Utility.CustomType;
using UnityEngine;

namespace Core
{
    // public partial class ArchmageNormalAttack : ActiveAbility
    // {
    //     [field: SerializeField] private DamageType DamageType { get; set; } = DamageType.Physical;
    //     [field: SerializeField] private Projectile Projectile {get; set;}
    //     [field: SerializeField] private Transform SpawnPosition {get; set;}
    //
    //     private Enemy EnemyTarget { get; set; }
    //     private Archmage OwnerArchmage { get; set; }
    //     
    //     public ArchmageNormalAttack(Hero owner, Projectile projectile, Transform spawnPosition) : base(owner)
    //     {
    //         Projectile = projectile;
    //         SpawnPosition = spawnPosition;
    //         OwnerArchmage = (Archmage) owner;
    //     }
    //
    //     public override void OnEnterBattleField()
    //     {
    //         
    //     }
    //
    //     public override void OnExitBattleField()
    //     {
    //         
    //     }
    //
    //     public override bool CanUseAbility()
    //     {
    //         if (!BattleManager.TryGetEnemyInAttackRange(Owner, out Enemy target)) return false;
    //         EnemyTarget = target;
    //         return true;
    //     }
    //
    //     public override async UniTask UseAbility(TickRate tickRate, CancellationToken ct)
    //     {
    //         BigNumber damageDeal = Owner.AttackDamage;
    //         float attackSpeed = Owner.AttackSpeed;
    //         EnemyTarget.WillTakeDamage(damageDeal);
    //         Owner.SetFacingPosition(EnemyTarget.Transform.position);
    //         Owner.HeroAnim.PlayAttackAnimation(attackSpeed);
    //         
    //         await DelaySample(8, tickRate, ct);
    //         Projectile.Spawn(SpawnPosition.position, Quaternion.identity)
    //             .Setup(Owner, EnemyTarget, damageDeal, DamageType)
    //             .WithComplete(OnProjectileMoveCompleteCache);
    //         if (Random.Range(0, 100) < OwnerArchmage.OrbSpawnRate)
    //         {
    //             OwnerArchmage.SpawnRandomOrb();
    //         }
    //         
    //         await DelaySample(22, tickRate, ct);
    //     }
    //     [ACacheMethod("TW.Utility.CustomType")]
    //     private void OnProjectileMoveComplete(Hero ownerHero, Enemy targetEnemy, BigNumber damage, DamageType damageType)
    //     {
    //         targetEnemy.TakeDamage(damage, damageType);
    //     }
    //
    //     public override Ability Clone()
    //     {
    //         return new ArchmageNormalAttack(Owner, Projectile, SpawnPosition);
    //     }
    // }
}