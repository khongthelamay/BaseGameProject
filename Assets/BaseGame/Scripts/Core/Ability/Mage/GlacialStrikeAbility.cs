using System.Threading;
using Core.SimplePool;
using Cysharp.Threading.Tasks;
using TW.Utility.CustomType;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "GlacialStrikeAbility", menuName = "Ability/Mage/GlacialStrikeAbility")]
    public class GlacialStrikeAbility : ActiveCooldownAbility, IAbilityTargetAnEnemy
    {
        [field: SerializeField] public DamageType DamageType {get; private set;}
        [field: SerializeField] public int DelayFrame {get; private set;}
        [field: SerializeField] public float DamageScale {get; private set;}
        [field: SerializeField] public float StunDuration {get; private set;}
        [field: SerializeField] public GlacialStrikeArea GlacialStrikeArea {get; private set;}
        public Enemy EnemyTarget { get; set; }
        public int EnemyTargetId { get; set; }
        public BigNumber FinalDamage { get; set; }

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
            BigNumber attackDamage = Owner.AttackDamage(out bool isCritical) * DamageScale;
            float attackSpeed = Owner.AttackSpeed;

            if (!EnemyTarget.WillTakeDamage(EnemyTargetId, attackDamage, DamageType, out BigNumber finalDamage)) return;
            FinalDamage = finalDamage;
            Owner.SetFacingPosition(EnemyTarget.Transform.position);
            Owner.HeroAnim.PlaySkill1Animation(attackSpeed);
            await DelaySample(DelayFrame, tickRate, ct);
            GlacialStrikeArea.Spawn(EnemyTarget.Transform.position, Quaternion.identity)
                .Setup(DamageType, 1.5f, 5f, 0.2f, FinalDamage, isCritical)
                .WithAxis(EnemyTarget.MoveAxis)
                .As<GlacialStrikeArea>()
                .WithStunDuration(StunDuration)
                .StartDamageOverTimeHandle();
            await DelaySample(30 - DelayFrame, tickRate, ct);
        }

        public void ChangeStunDuration(float duration)
        {
            StunDuration += duration;
        }
    }
}