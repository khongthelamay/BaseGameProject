﻿using System.Threading;
using Core.SimplePool;
using Cysharp.Threading.Tasks;
using TW.ACacheEverything;
using TW.Utility.CustomType;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "SavageBlastAbility", menuName = "Ability/Boar/SavageBlastAbility")]
    public partial class SavageBlastAbility : CooldownAbility, IAbilityTargetEnemiesInRange
    {
        [field: SerializeField] public DamageType DamageType {get; private set;}
        [field: SerializeField] public int DelayFrame {get; private set;}
        [field: SerializeField] public float DamageScale {get; private set;}
        [field: SerializeField] public Projectile Projectile {get; private set;}
        [field: SerializeField] public VisualEffect VisualEffect {get; private set;}
        [field: SerializeField] public AxisDetection<Vector2> Range { get; set; }

        private Transform SpawnPosition {get; set;}
        public Enemy EnemyTarget { get; set; }
        public int EnemyTargetId { get; set; }
        public Enemy[] Enemies { get; set; } = new Enemy[30];
        public int[] EnemiesTargetId { get; set; } = new int[30];
        public int EnemiesCount { get; set; }
        private Boar OwnerBoar { get; set; }

        public override Ability WithOwnerHero(Hero owner)
        {
            OwnerBoar = owner as Boar;
            SpawnPosition = OwnerBoar?.ProjectileSpawnPosition;

            return base.WithOwnerHero(owner);
        }

        public override bool CanUseAbility()
        {
            if (IsOnCooldown) return false;
            return this.IsFindAnyEnemyTarget();
        }

        public override async UniTask UseAbility(TickRate tickRate, CancellationToken ct)
        {
            ResetCooldown();
            BigNumber attackDamage = Owner.AttackDamage(out bool isCritical) * DamageScale;
            float attackSpeed = Owner.AttackSpeed;
            
            Owner.SetFacingPosition(EnemyTarget.Transform.position);
            Owner.HeroAnim.PlaySkill2Animation(attackSpeed);

            await DelaySample(DelayFrame, tickRate, ct);
            Projectile.Spawn(SpawnPosition.position, Quaternion.identity)
                .Setup(Owner, EnemyTarget, EnemyTargetId, attackDamage, DamageType, isCritical)
                .WithComplete(OnProjectileMoveCompleteCache);
            await DelaySample(30 - DelayFrame, tickRate, ct);
        }
        [ACacheMethod("TW.Utility.CustomType")]
        private void OnProjectileMoveComplete(Hero ownerHero, Enemy targetEnemy, int targetEnemyId, BigNumber attackDamage, DamageType damageType, bool isCritical)
        {
            VisualEffect.Spawn(targetEnemy.Transform.position, Quaternion.identity).Play();
            Enemy[] enemies = new Enemy[30];
            int[] enemiesTargetId = new int[30];
            int enemiesCount = BattleManager.GetEnemyAroundNonAlloc(targetEnemy.Transform.position, Range.Current, enemies);
            if (enemiesCount == 0) return;
            for (int i = 0; i < enemiesCount; i++)
            {
                enemiesTargetId[i] = enemies[i].Id;
            }
            for (int i = 0; i < enemiesCount; i++)
            {
                enemies[i].TakeDamage(enemiesTargetId[i], attackDamage, DamageType ,isCritical);
            }
            
        }


    }
}