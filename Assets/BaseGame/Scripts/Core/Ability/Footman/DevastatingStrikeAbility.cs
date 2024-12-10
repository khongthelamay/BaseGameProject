using System.Threading;
using Core.SimplePool;
using Cysharp.Threading.Tasks;
using Manager;
using TW.Utility.CustomType;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "DevastatingStrikeAbility", menuName = "Ability/Footman/DevastatingStrikeAbility")]
    public class DevastatingStrikeAbility : ActiveAbility, IAbilityTargetAnEnemy
    {
        [field: SerializeField] public int DevastatingStrikeRate {get; private set;}
        [field: SerializeField] public DamageType DamageType {get; private set;}
        [field: SerializeField] public int DamageDelayFrame {get; private set;}
        [field: SerializeField] public int VisualDelayFrame {get; private set;}
        [field: SerializeField] public float DamageScale {get; private set;}
        [field: SerializeField] public VisualEffect VisualEffect {get; private set;}

        public Enemy EnemyTarget { get; set; }
        public int EnemyTargetId { get; set; }
        public override bool CanUseAbility()
        {
            if (Random.Range(0, 100) >= DevastatingStrikeRate) return false;
            return this.IsFindAnyEnemyTarget();
        }
    
        public override void OnEnterBattleField()
        {
            
        }
    
        public override void OnExitBattleField()
        {
            
        }
        
        public override async UniTask UseAbility(TickRate tickRate, CancellationToken ct)
        {
            BigNumber damageDeal = Owner.AttackDamage(out bool isCritical) * DamageScale;
            float attackSpeed = Owner.AttackSpeed;
            
            if (!EnemyTarget.WillTakeDamage(EnemyTargetId, damageDeal)) return;
            Owner.SetFacingPosition(EnemyTarget.Transform.position);
            Owner.HeroAnim.PlaySkill1Animation(attackSpeed);
            await DelaySample(DamageDelayFrame - VisualDelayFrame, tickRate, ct);
            VisualEffect.Spawn(EnemyTarget.Transform.position, Quaternion.identity);
            await DelaySample(VisualDelayFrame, tickRate, ct);
            if (!EnemyTarget.TakeDamage(EnemyTargetId, damageDeal, DamageType, isCritical)) return;
            await DelaySample(30 - DamageDelayFrame, tickRate, ct);
        }


    }
}