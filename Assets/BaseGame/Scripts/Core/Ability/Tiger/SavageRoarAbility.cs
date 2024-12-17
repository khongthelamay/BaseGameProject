﻿using System.Threading;
using Core.SimplePool;
using Cysharp.Threading.Tasks;
using TW.Utility.CustomType;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;

namespace Core.TigerAbility
{
    [CreateAssetMenu(fileName = "SavageRoarAbility", menuName = "Ability/Tiger/SavageRoarAbility")]
    public class SavageRoarAbility : ActiveCooldownAbility, IAbilityTargetAroundEnemy
    {
        [field: SerializeField] public DamageType DamageType { get; private set; } 
        [field: SerializeField] public int DamageScale { get; private set; }
        [field: SerializeField] public VisualEffect VisualEffect { get; private set; }

        public float Radius { get; } = 2f;
        public Enemy EnemyTarget { get; set; }
        public Enemy[] Enemies { get; set; } = new Enemy[30];
        public int[] EnemiesTargetId { get; set; } = new int[30];
        public int EnemiesCount { get; set; }
        public Tiger OwnerTiger { get; set; }

        public override Ability WithOwnerHero(Hero owner)
        {
            OwnerTiger = owner as Tiger;
            return base.WithOwnerHero(owner);
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
            return this.IsFindAnyEnemyTarget();
        }
    
        public override async UniTask UseAbility(TickRate tickRate, CancellationToken ct)
        {
            ResetCooldown();
            BigNumber damageDeal = Owner.AttackDamage(out bool isCritical) * DamageScale;
            float attackSpeed = Owner.AttackSpeed;
            
            for (int i = 0; i < EnemiesCount; i++)
            {
                Enemies[i].WillTakeDamage(EnemiesTargetId[i], damageDeal);
            }
            Owner.SetFacingPosition(EnemyTarget.Transform.position);
            Owner.HeroAnim.PlaySkill1Animation(attackSpeed);
            OwnerTiger.ChangeFuryPoint(1);
            await DelaySample(17, tickRate, ct);
            
            VisualEffect.Spawn(EnemyTarget.transform.position, Quaternion.identity);
            await DelaySample(2, tickRate, ct);
            for (int i = 0; i < EnemiesCount; i++)
            {
                Enemies[i].TakeDamage(EnemiesTargetId[i], damageDeal, DamageType ,isCritical);
            }
            await DelaySample(11, tickRate, ct);

        }


    }
}