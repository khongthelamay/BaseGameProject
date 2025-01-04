using System.Threading;
using Core.GameStatusEffect;
using Core.SimplePool;
using Cysharp.Threading.Tasks;
using LitMotion;
using TW.ACacheEverything;
using TW.Utility.CustomType;
using TW.Utility.Extension;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "HowlOfRageAbility", menuName = "Ability/Wolf/HowlOfRageAbility")]
    public class HowlOfRageAbility : RandomnessAbility , IAbilityTargetAnEnemy
    {
        [field: SerializeField] public DamageType DamageType {get; private set;}
        [field: SerializeField] public int CriticalRateBonus {get; private set;}
        [field: SerializeField] public float CriticalRateBonusDuration {get; private set;}
        [field: SerializeField] public int DelayFrame {get; private set;}
        [field: SerializeField] public VisualEffect VisualEffect {get; private set;}

        public Enemy EnemyTarget { get; set; }
        public int EnemyTargetId { get; set; }

        public override void OnEnterBattleField()
        {
            
        }

        public override void OnExitBattleField()
        {
            
        }
        public override bool CanUseAbility()
        {
            return base.CanUseAbility() && this.IsFindAnyEnemyTarget();
        }

        public override async UniTask UseAbility(TickRate tickRate, CancellationToken ct)
        {
            BigNumber attackDamage = Owner.AttackDamage(out bool isCritical);
            float attackSpeed = Owner.AttackSpeed;
            
            Owner.SetFacingPosition(EnemyTarget.Transform.position);
            Owner.HeroAnim.PlaySkill2Animation(attackSpeed);
            await DelaySample(DelayFrame, tickRate, ct);
            if (!EnemyTarget.TakeDamage(EnemyTargetId, attackDamage, DamageType, isCritical)) return;
            VisualEffect.Spawn(EnemyTarget.Transform.position, Quaternion.identity, EnemyTarget.Transform)
                .WithLocalScale(1)
                .Play();
            Owner.AddStatusEffect(new CriticalChanceChangeEffect(CriticalRateBonus, CriticalRateBonusDuration));
            await DelaySample(30 - DelayFrame, tickRate, ct);
        }



    }
}