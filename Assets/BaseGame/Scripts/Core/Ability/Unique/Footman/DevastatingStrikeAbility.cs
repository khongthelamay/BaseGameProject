﻿using System.Threading;
using Core.SimplePool;
using Cysharp.Threading.Tasks;
using Manager;
using TW.Utility.CustomType;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "DevastatingStrikeAbility", menuName = "Ability/Footman/DevastatingStrikeAbility")]
    public class DevastatingStrikeAbility : RandomnessAbility, IAbilityTargetAnEnemy
    {
        [field: SerializeField] public DamageType DamageType {get; private set;}
        [field: SerializeField] public int DamageDelayFrame {get; private set;}
        [field: SerializeField] public int VisualDelayFrame {get; private set;}
        [field: SerializeField] public float DamageScale {get; private set;}
        [field: SerializeField] public VisualEffect VisualEffect {get; private set;}

        public Enemy EnemyTarget { get; set; }
        public int EnemyTargetId { get; set; }

        public override bool CanUseAbility()
        {
            return base.CanUseAbility() && this.IsFindAnyEnemyTarget();
        }
    
        public override void OnEnterBattleField()
        {
            
        }
    
        public override void OnExitBattleField()
        {
            
        }
        
        public override async UniTask UseAbility(TickRate tickRate, CancellationToken ct)
        {
            BigNumber attackDamage = Owner.AttackDamage(out bool isCritical) * DamageScale;
            float attackSpeed = Owner.AttackSpeed;

            Owner.SetFacingPosition(EnemyTarget.Transform.position);
            Owner.HeroAnim.PlaySkill1Animation(attackSpeed);
            await DelaySample(DamageDelayFrame - VisualDelayFrame, tickRate, ct);
            VisualEffect.Spawn(EnemyTarget.Transform.position, Quaternion.identity).Play();
            await DelaySample(VisualDelayFrame, tickRate, ct);
            if (!EnemyTarget.TakeDamage(EnemyTargetId, attackDamage, DamageType, isCritical)) return;
            await DelaySample(30 - DamageDelayFrame, tickRate, ct);
        }


    }
}