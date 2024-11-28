using System.Threading;
using Core.SimplePool;
using Cysharp.Threading.Tasks;
using TW.Utility.CustomType;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;

namespace Core.TigerAbility
{
    [System.Serializable]
    public class SavageRoarAbility : ActiveCooldownAbility
    {
        [field: SerializeField] private DamageType DamageType { get; set; } = DamageType.Physical; 
        [field: SerializeField] private int DamageScale { get; set; } = 100;
        private VisualEffect VisualEffect { get; set; }
        private Enemy EnemyTarget { get; set; }
        private Enemy[] Enemies { get; set; } = new Enemy[30]; 
        private int EnemiesCount { get; set; }
        private Tiger OwnerTiger { get; set; }
    
        public override void OnEnterBattleField()
        {
            StartCooldownHandle();
        }
    
        public override void OnExitBattleField()
        {
            StopCooldownHandle();
        }

        // public override Ability Create()
        // {
        //     return ScriptableObject.CreateInstance<SavageRoarAbility>();
        // }

        public override bool CanUseAbility()
        {
            if (IsOnCooldown) return false;
            if (!BattleManager.TryGetEnemyInAttackRange(Owner, out Enemy target)) return false;
            EnemyTarget = target;
            EnemiesCount = BattleManager.GetEnemyAroundNonAlloc(EnemyTarget.Transform.position, 2, Enemies);
            return true;
        }
    
        public override async UniTask UseAbility(TickRate tickRate, CancellationToken ct)
        {
            CooldownTimer = Cooldown;
            BigNumber damageDeal = Owner.AttackDamage * DamageScale;
            float attackSpeed = Owner.AttackSpeed;
            
            for (int i = 0; i < EnemiesCount; i++)
            {
                Enemies[i].WillTakeDamage(damageDeal);
            }
            Owner.SetFacingPosition(EnemyTarget.Transform.position);
            Owner.HeroAnim.PlaySkillAnimation(attackSpeed);
            OwnerTiger.ChangeFuryPoint(1);
            await DelaySample(17, tickRate, ct);
    
            VisualEffect.Spawn(EnemyTarget.transform.position, Quaternion.identity);
            await DelaySample(2, tickRate, ct);
            for (int i = 0; i < EnemiesCount; i++)
            {
                Enemies[i].TakeDamage(damageDeal, DamageType);
            }
            await DelaySample(11, tickRate, ct);
            ResetCooldown();
        }
    }
}