using System.Threading;
using Core.SimplePool;
using Cysharp.Threading.Tasks;
using TW.Utility.CustomType;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;

namespace Core
{
    public class ElementalSurgeAbility : ActiveCooldownAbility
    {
        [field: SerializeField] private DamageType DamageType { get; set; } = DamageType.Magical;
        [field: SerializeField] private VisualEffect VisualEffect { get; set; }
        private Enemy EnemyTarget { get; set; }
        private Enemy[] Enemies { get; set; } = new Enemy[30]; 
        private int EnemiesCount { get; set; }
        private Archmage OwnerArchmage { get; set; }
        
        public ElementalSurgeAbility(Hero owner, int levelUnlock, float cooldown, VisualEffect visualEffect) : base(owner, levelUnlock, cooldown)
        {
            VisualEffect = visualEffect;
            OwnerArchmage = (Archmage) owner;
        }

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
            if (!BattleManager.TryGetEnemyInAttackRange(Owner, out Enemy target)) return false;
            EnemyTarget = target;
            EnemiesCount = BattleManager.GetEnemyAroundNonAlloc(EnemyTarget.Transform.position, 2, Enemies);
            return true;
        }

        public override async UniTask UseAbility(TickRate tickRate, CancellationToken ct)
        {
            CooldownTimer = Cooldown;
            BigNumber damageDeal = Owner.AttackDamage * OwnerArchmage.FireOrbCount;
            float attackSpeed = Owner.AttackSpeed;
            
            for (int i = 0; i < EnemiesCount; i++)
            {
                Enemies[i].WillTakeDamage(damageDeal);
            }
            
            Owner.SetFacingPosition(EnemyTarget.Transform.position);
            Owner.HeroAnim.PlaySkillAnimation(attackSpeed);
            OwnerArchmage.ConsumeAllOrb();
            
            await DelaySample(17, tickRate, ct);
            VisualEffect.Spawn(EnemyTarget.Transform.position, Quaternion.identity);
            await DelaySample(6, tickRate, ct);

            for (int i = 0; i < EnemiesCount; i++)
            {
                Enemies[i].TakeDamage(damageDeal, DamageType);
            }
            await DelaySample(7, tickRate, ct);
            ResetCooldown();
        }
    }
}