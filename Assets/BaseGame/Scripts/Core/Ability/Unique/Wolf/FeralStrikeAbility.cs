﻿using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.CustomType;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "FeralStrikeAbility", menuName = "Ability/Wolf/FeralStrikeAbility")]
    public class FeralStrikeAbility : RandomnessAbility, IAbilityTargetAnEnemy
    {
        [field: SerializeField] public DamageType DamageType { get; private set; }
        [field: SerializeField] public int DelayFrame { get; private set; } 
        [field: SerializeField] public float DamageScale { get; private set; }

        public Enemy EnemyTarget { get; set; }
        public int EnemyTargetId { get; set; }

        public override bool CanUseAbility()
        {
            return base.CanUseAbility() && this.IsFindAnyEnemyTarget();
        }
        public override async UniTask UseAbility(TickRate tickRate, CancellationToken ct)
        {
            BigNumber attackDamage = Owner.AttackDamage(out bool isCritical) * DamageScale;
            float attackSpeed = Owner.AttackSpeed;

            Owner.SetFacingPosition(EnemyTarget.Transform.position);
            Owner.HeroAnim.PlaySkill1Animation(attackSpeed);

            await DelaySample(DelayFrame, tickRate, ct);
            if (!EnemyTarget.TakeDamage(EnemyTargetId, attackDamage, DamageType, isCritical)) return;
            await DelaySample(30 - DelayFrame, tickRate, ct);
        }

        public override void OnEnterBattleField()
        {
        }

        public override void OnExitBattleField()
        {
        }
    }
}