using System.Numerics;
using System.Threading;
using BaseGame.Scripts.Enum;
using Core.SimplePool;
using Cysharp.Threading.Tasks;
using LitMotion;
using Manager;
using TW.ACacheEverything;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;

namespace Core
{
    public partial class SavageRoarAbility : ActiveCooldownAbility
    {
        [field: SerializeField] private int DamageScale { get; set; }
        [field: SerializeField] private DamageType DamageType { get; set; }
        [field: SerializeField] private VisualEffect VisualEffect { get; set; }
        private Enemy EnemyTarget { get; set; }
        private Enemy[] Enemies { get; set; } = new Enemy[30]; 
        private int EnemiesCount { get; set; }
        private Tiger OwnerTiger { get; set; }
        public SavageRoarAbility(Hero owner, int levelUnlock, float cooldown, int damageScale, DamageType damageType, VisualEffect visualEffect) : base(owner, levelUnlock, cooldown)
        {
            DamageScale = damageScale;
            DamageType = damageType;
            VisualEffect = visualEffect;
            OwnerTiger = (Tiger) owner;
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
            Owner.HeroAnim.PlaySkillAnimation(Owner.AttackSpeed);
            OwnerTiger.AddFuryPoint(1);
            await DelaySample(17, tickRate, ct);
            BigInteger attackDamage = (BigInteger)(Owner.AttackDamage * (1 + BattleManager.GetGlobalBuff(GlobalBuff.Type.AttackDamage).Value/100));
            BigInteger damage = attackDamage * DamageScale;
            VisualEffect.Spawn(EnemyTarget.transform.position, Quaternion.identity);
            await DelaySample(2, tickRate, ct);
            for (int i = 0; i < EnemiesCount; i++)
            {
                Enemies[i].TakeDamage(damage, DamageType);
            }
            await DelaySample(11, tickRate, ct);
            StartCooldownHandle();
        }
    }
}