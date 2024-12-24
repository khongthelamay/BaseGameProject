using System.Threading;
using Core.GameStatusEffect;
using Cysharp.Threading.Tasks;
using TW.Utility.CustomType;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "RoaringStrengthAbility", menuName = "Ability/Bear/RoaringStrengthAbility")]
    public class RoaringStrengthAbility : ActiveAbility, IAbilityTargetAnEnemy, IAbilityTargetAlliesInRange
    {
        [field: SerializeField] public int RoaringStrengthRate {get; private set;}
        [field: SerializeField] public DamageType DamageType {get; private set;}
        [field: SerializeField] public int DamageDelayFrame {get; private set;}
        [field: SerializeField] public float DamageScale {get; private set;}
        [field: SerializeField] public float AttackDamageBuff {get; private set;}
        [field: SerializeField] public float BuffDuration {get; private set;}
        [field: SerializeField] public AxisDetection<Vector2> AlliesRange { get; set; }
        public Hero[] Heroes { get; set; } = new Hero[30];
        public int[] HeroesId { get; set; } = new int[30];
        public int HeroesCount { get; set; }
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
            if (Random.Range(0, 100) >= RoaringStrengthRate) return false;
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
            if (!this.IsFindAnyAlliesTarget()) return;
            for (int i = 0; i < HeroesCount; i++)
            {
                if (Heroes[i].Id == HeroesId[i])
                {
                    Heroes[i].AddStatusEffect(new AttackDamageChangeEffect(AttackDamageBuff, BuffDuration));
                }
            }
        }


    }
}