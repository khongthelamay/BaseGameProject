using System.Threading;
using Core.GameStatusEffect;
using Cysharp.Threading.Tasks;
using TW.Utility.CustomType;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "JudgmentStrikeAbility", menuName = "Ability/Knight/JudgmentStrikeAbility")]
    public class JudgmentStrikeAbility : ActiveAbility, IAbilityTargetAnEnemy
    {
        [field: SerializeField] public int JudgmentStrikeRate {get; private set;}
        [field: SerializeField] public DamageType DamageType {get; private set;}
        [field: SerializeField] public int DamageDelayFrame {get; private set;}
        [field: SerializeField] public float DamageScale {get; private set;}
        [field: SerializeField] public int ArmorReduce {get; private set;}
        [field: SerializeField] public float ArmorReduceDuration {get; private set;}
        public Enemy EnemyTarget { get; set; }
        public int EnemyTargetId { get; set; }
        public BigNumber FinalDamage { get; set; }

        public override void OnEnterBattleField()
        {
        }

        public override void OnExitBattleField()
        {
        }

        public override bool CanUseAbility()
        {
            if (Random.Range(0, 100) >= JudgmentStrikeRate) return false;
            return this.IsFindAnyEnemyTarget();
        }

        public override async UniTask UseAbility(TickRate tickRate, CancellationToken ct)
        {
            BigNumber attackDamage = Owner.AttackDamage(out bool isCritical) * DamageScale;
            float attackSpeed = Owner.AttackSpeed;
            if (!EnemyTarget.WillTakeDamage(EnemyTargetId, attackDamage, DamageType, out BigNumber finalDamage)) return;
            FinalDamage = finalDamage;
            Owner.SetFacingPosition(EnemyTarget.Transform.position);
            Owner.HeroAnim.PlaySkill1Animation(attackSpeed);
            await DelaySample(DamageDelayFrame, tickRate, ct);
            if (!EnemyTarget.TakeDamage(EnemyTargetId, FinalDamage, DamageType, isCritical)) return;
            OnAttackComplete();
            await DelaySample(30 - DamageDelayFrame, tickRate, ct);
        }
        private void OnAttackComplete()
        {
            if (EnemyTarget.Id == EnemyTargetId)
            {
                EnemyTarget.AddStatusEffect(new PhysicalArmorChangeEffect(ArmorReduce, ArmorReduceDuration));
            }
        }
    }
}