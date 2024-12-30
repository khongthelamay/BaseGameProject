using System.Threading;
using Core.GameStatusEffect;
using Cysharp.Threading.Tasks;
using TW.Utility.CustomType;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "StaticChargeAbility", menuName = "Ability/ThunderLizard/StaticChargeAbility")]
    public class StaticChargeAbility : ActiveAbility, IAbilityTargetAnEnemy
    {
        [field: SerializeField] public int StaticChargeRate {get; private set;}
        [field: SerializeField] public DamageType DamageType {get; private set;}
        [field: SerializeField] public int DelayFrame {get; private set;}
        [field: SerializeField] public float DamageScale {get; private set;}
        [field: SerializeField] public int CriticalDamageBuff {get; private set;}
        [field: SerializeField] public float CriticalDamageBuffDuration {get; private set;}
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
            if (Random.Range(0, 100) >= StaticChargeRate) return false;
            return this.IsFindAnyEnemyTarget();
        }

        public override async UniTask UseAbility(TickRate tickRate, CancellationToken ct)
        {
            BigNumber attackDamage = Owner.AttackDamage(out bool isCritical) * DamageScale;
            float attackSpeed = Owner.AttackSpeed;

            Owner.SetFacingPosition(EnemyTarget.Transform.position);
            Owner.HeroAnim.PlaySkill1Animation(attackSpeed);

            await DelaySample(DelayFrame, tickRate, ct);
            if (!EnemyTarget.TakeDamage(EnemyTargetId, attackDamage, DamageType, isCritical)) return;
            Owner.AddStatusEffect(new CriticalDamageChangeEffect(CriticalDamageBuff, CriticalDamageBuffDuration));
            await DelaySample(30 - DelayFrame, tickRate, ct);
        }
    }
}