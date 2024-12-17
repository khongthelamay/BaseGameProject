﻿using System.Threading;
using Core.SimplePool;
using Cysharp.Threading.Tasks;
using LitMotion;
using TW.ACacheEverything;
using TW.Utility.CustomType;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "ArrowStormAbility", menuName = "Ability/Archer/ArrowStormAbility")]
    public partial class ArrowStormAbility : ActiveAbility, IAbilityTargetAroundEnemy
    {
        [field: SerializeField] public int ArrowStormRate {get; private set;}
        [field: SerializeField] public DamageType DamageType {get; private set;}
        [field: SerializeField] public int DelayFrame {get; private set;}
        [field: SerializeField] public float DamageScale {get; private set;}

        [field: SerializeField] public Projectile Projectile {get; private set;}
        [field: SerializeField] public VisualEffect VisualEffect {get; private set;}

        private Transform SpawnPosition {get; set;}
        public float Radius { get; } = 2f;
        public Enemy EnemyTarget { get; set; }
        public Enemy[] Enemies { get; set; } = new Enemy[30];
        public int[] EnemiesTargetId { get; set; } = new int[30];
        public int EnemiesCount { get; set; }
        private Archer OwnerArcher { get; set; }

        public override Ability WithOwnerHero(Hero owner)
        {
            OwnerArcher = owner as Archer;
            SpawnPosition = OwnerArcher?.ProjectileSpawnPosition;

            return base.WithOwnerHero(owner);
        }
        public override void OnEnterBattleField()
        {
            
        }

        public override void OnExitBattleField()
        {
            
        }

        public override bool CanUseAbility()
        {
            if (Random.Range(0, 100) >= ArrowStormRate) return false;
            return this.IsFindAnyEnemyTarget();
        }

        public override async UniTask UseAbility(TickRate tickRate, CancellationToken ct)
        {
            BigNumber damageDeal = Owner.AttackDamage(out bool isCritical) * DamageScale;
            float attackSpeed = Owner.AttackSpeed;

            for (int i = 0; i < EnemiesCount; i++)
            {
                Enemies[i].WillTakeDamage(EnemiesTargetId[i], damageDeal);
            }
            Owner.SetFacingPosition(EnemyTarget.Transform.position);
            Owner.HeroAnim.PlaySkill1Animation(attackSpeed);
            await DelaySample(DelayFrame, tickRate, ct);
            VisualEffect.Spawn(EnemyTarget.Transform.position, Quaternion.identity)
                .WithSpeed(attackSpeed)
                .WithAxis(EnemyTarget.MoveAxis > 0 ? -1 : 1);
            await DelaySample(3, tickRate, ct);
            for (int i = 0; i < EnemiesCount; i++)
            {
                Enemies[i].TakeDamage(EnemiesTargetId[i], damageDeal, DamageType ,isCritical);
            }
            await DelaySample(30 - DelayFrame -3, tickRate, ct);
        }
    }
}