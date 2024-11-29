using System.Threading;
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
        [field: SerializeField] public int DelayFrame {get; private set;}
        private MotionHandle DurationMotionHandle { get; set; }
        public Enemy EnemyTarget { get; set; }
        public int EnemyTargetId { get; set; }
        public override void OnEnterBattleField()
        {
            DurationMotionHandle.TryCancel();
        }

        public override void OnExitBattleField()
        {
            DurationMotionHandle.TryCancel();
        }

        public override bool CanUseAbility()
        {
            if (Random.Range(0, 100) >= HowlOfRageRate) return false;
            return this.IsFindAnyEnemyTarget();
        }

        public override async UniTask UseAbility(TickRate tickRate, CancellationToken ct)
        {

            
            BigNumber damageDeal = Owner.AttackDamage(out bool isCritical);
            float attackSpeed = Owner.AttackSpeed;
            
            if (!EnemyTarget.WillTakeDamage(EnemyTargetId, damageDeal)) return;
            Owner.SetFacingPosition(EnemyTarget.Transform.position);
            Owner.HeroAnim.PlayAttackAnimation(attackSpeed);
            await DelaySample(DelayFrame, tickRate, ct);
            if (!EnemyTarget.TakeDamage(EnemyTargetId, damageDeal, DamageType, isCritical)) return;
            StartDuration();
            await DelaySample(30 - DelayFrame, tickRate, ct);
        }
        public void StartDuration()
        {
            if (!DurationMotionHandle.IsActive())
            {
                Owner.ChangeCriticalRate(CriticalRateBonus);
            }
            else
            {
                DurationMotionHandle.Cancel();
            }

            DurationMotionHandle = LMotion.Create(0, 1, 3)
                .WithOnComplete(OnEndDurationCache)
                .RunWithoutBinding();
        }
        [ACacheMethod]
        private void OnEndDuration()
        {
            Owner.ChangeCriticalRate(-CriticalRateBonus);
        }


    }
}