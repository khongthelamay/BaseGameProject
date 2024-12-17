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
        [field: SerializeField] public DamageOverTimeArea GlacialStrikeArea {get; private set;}
        public Enemy EnemyTarget { get; set; }
        public int EnemyTargetId { get; set; }
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

            if (!EnemyTarget.WillTakeDamage(EnemyTargetId,damageDeal)) return;
            Owner.SetFacingPosition(EnemyTarget.Transform.position);
            Owner.HeroAnim.PlaySkill1Animation(attackSpeed);
            await DelaySample(DelayFrame, tickRate, ct);
            GlacialStrikeArea.Spawn(EnemyTarget.Transform.position, Quaternion.identity)
                .Setup(DamageType, 1f, 5f, 0.2f, damageDeal, isCritical)
                .WithAxis(EnemyTarget.MoveAxis);
            await DelaySample(30 - DelayFrame, tickRate, ct);
        }


    }
}