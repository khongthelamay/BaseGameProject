using System.Threading;
using Core.GameStatusEffect;
using Core.SimplePool;
using Cysharp.Threading.Tasks;
using LitMotion;
using TW.ACacheEverything;
using TW.Utility.CustomType;
using TW.Utility.Extension;
using UnityEngine;

namespace Core.WolfAbility
{
    [CreateAssetMenu(fileName = "HowlOfRageAbility", menuName = "Ability/Wolf/HowlOfRageAbility")]
    public partial class HowlOfRageAbility : ActiveAbility , IAbilityTargetAnEnemy
    {
        [field: SerializeField] public int HowlOfRageRate {get; private set;}
        [field: SerializeField] public DamageType DamageType {get; private set;}
        [field: SerializeField] public int CriticalRateBonus {get; private set;}
        [field: SerializeField] public float CriticalRateBonusDuration {get; private set;}
        [field: SerializeField] public int DelayFrame {get; private set;}
        [field: SerializeField] public VisualEffect VisualEffect {get; private set;}

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
            if (Random.Range(0, 100) >= HowlOfRageRate) return false;
            return this.IsFindAnyEnemyTarget();
        }

        public override async UniTask UseAbility(TickRate tickRate, CancellationToken ct)
        {
            BigNumber attackDamage = Owner.AttackDamage(out bool isCritical);
            float attackSpeed = Owner.AttackSpeed;
            
            if (!EnemyTarget.WillTakeDamage(EnemyTargetId, attackDamage, DamageType, out BigNumber finalDamage)) return;
            FinalDamage = finalDamage;
            Owner.SetFacingPosition(EnemyTarget.Transform.position);
            Owner.HeroAnim.PlaySkill2Animation(attackSpeed);
            await DelaySample(DelayFrame, tickRate, ct);
            if (!EnemyTarget.TakeDamage(EnemyTargetId, FinalDamage, DamageType, isCritical)) return;
            VisualEffect.Spawn(EnemyTarget.Transform.position, Quaternion.identity, EnemyTarget.Transform)
                .WithLocalScale(1)
                .Play();
            Owner.AddStatusEffect(new CriticalChanceChangeEffect(CriticalRateBonus, CriticalRateBonusDuration));
            await DelaySample(30 - DelayFrame, tickRate, ct);
        }



    }
}