using System.Threading;
using Core.SimplePool;
using Cysharp.Threading.Tasks;
using TW.Utility.CustomType;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "LightningFuryAbility", menuName = "Ability/ThunderLizard/LightningFuryAbility")]
    public class LightningFuryAbility : ActiveCooldownAbility, IAbilityTargetEnemiesInRange
    {
        [field: SerializeField] public DamageType DamageType {get; private set;}
        [field: SerializeField] public int DelayFrame {get; private set;}
        [field: SerializeField] public float DamageScale {get; private set;}
        [field: SerializeField] public VisualEffect VisualEffect {get; private set;}

        [field: SerializeField] public AxisDetection<Vector2> Range { get; set; }
        public Enemy EnemyTarget { get; set; }
        public int EnemyTargetId { get; set; }
        public Enemy[] Enemies { get; set; } = new Enemy[30];
        public int[] EnemiesTargetId { get; set; } = new int[30];
        public int EnemiesCount { get; set; }
        
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
            Owner.SetFacingPosition(EnemyTarget.Transform.position);
            
            Owner.HeroAnim.PlaySkill3Animation(attackSpeed);
            await DelaySample(DelayFrame, tickRate, ct);
            VisualEffect.Spawn(EnemyTarget.Transform.position, Quaternion.identity)
                .WithSpeed(attackSpeed)
                .WithAxis(EnemyTarget.MoveAxis)
                .Play();
            for (int i = 0; i < EnemiesCount; i++)
            {
                Enemies[i].TakeDamage(EnemiesTargetId[i], attackDamage, DamageType ,isCritical);
            }
            await DelaySample(30 - DelayFrame, tickRate, ct);
        }


    }
}