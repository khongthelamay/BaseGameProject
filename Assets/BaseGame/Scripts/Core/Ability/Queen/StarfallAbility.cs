using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.CustomType;
using UnityEngine;

namespace Core
{
    public class StarfallAbility : ActiveCooldownAbility
    {
        [field: SerializeField] private int DamageScale { get; set; } = 100;
        private Enemy EnemyTarget { get; set; }
        private Enemy[] Enemies { get; set; } = new Enemy[30]; 
        private int EnemiesCount { get; set; }
        private Queen OwnerQueen { get; set; }
        public StarfallAbility(Hero owner, int levelUnlock, float cooldown) : base(owner, levelUnlock, cooldown)
        {
            OwnerQueen = owner as Queen;
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
            BigNumber damageDeal = Owner.AttackDamage * DamageScale;
            float attackSpeed = Owner.AttackSpeed;
            
            CooldownTimer = Cooldown;
            Owner.SetFacingPosition(EnemyTarget.Transform.position);
            Owner.HeroAnim.PlaySkillAnimation(attackSpeed);
            
            OwnerQueen.ReduceCooldownAllHeroAround(10);
            await DelaySample(30, tickRate, ct);
            ResetCooldown();
        }
    }
}