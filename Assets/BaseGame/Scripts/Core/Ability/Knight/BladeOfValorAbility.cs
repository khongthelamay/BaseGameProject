using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.CustomType;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "BladeOfValorAbility", menuName = "Ability/Knight/BladeOfValorAbility")]
    public class BladeOfValorAbility : ActiveCooldownAbility, IAbilityTargetAnEnemy
    {
        [field: SerializeField] public DamageType DamageType {get; private set;}
        [field: SerializeField] public int DamageDelayFrame {get; private set;}
        [field: SerializeField] public float DamageScale {get; private set;}
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
            Owner.HeroAnim.PlaySkill2Animation(attackSpeed);
            await DelaySample(DamageDelayFrame, tickRate, ct);
            if (!EnemyTarget.TakeDamage(EnemyTargetId, FinalDamage, DamageType, isCritical)) return;
            await DelaySample(30 - DamageDelayFrame, tickRate, ct);
        }


    }
}