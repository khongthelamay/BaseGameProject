using System.Threading;
using Core.GameStatusEffect;
using Core.SimplePool;
using Cysharp.Threading.Tasks;
using TW.Utility.CustomType;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "FrostNovaAbility", menuName = "Ability/Mage/FrostNovaAbility")]
    public partial class FrostNovaAbility : ActiveAbility, IAbilityTargetInRangeEnemy
    {
        [field: SerializeField] public int FrostNovaRate {get; private set;}
        [field: SerializeField] public DamageType DamageType {get; private set;}
        [field: SerializeField] public int DelayFrame {get; private set;}
        [field: SerializeField] public float DamageScale {get; private set;}
        [field: SerializeField] public VisualEffect VisualEffect {get; private set;}
        [field: SerializeField] public AxisDetection<Vector2> Range { get; set; }

        private Transform SpawnPosition {get; set;}
        
        public Enemy EnemyTarget { get; set; }
        public int EnemyTargetId { get; set; }
        public Enemy[] Enemies { get; set; } = new Enemy[30];
        public int[] EnemiesTargetId { get; set; } = new int[30];
        public int EnemiesCount { get; set; }
        private Mage OwnerMage { get; set; }
        public override Ability WithOwnerHero(Hero owner)
        {
            OwnerMage = owner as Mage;
            SpawnPosition = OwnerMage?.ProjectileSpawnPosition;

            return base.WithOwnerHero(owner);
        }
        public override void OnEnterBattleField()
        {
            
        }

        public override void OnExitBattleField()
        {
        }

        public override bool CanUseAbility()
        {
            if (Random.Range(0, 100) >= FrostNovaRate) return false;
            return this.IsFindAnyEnemyTarget();
        }

        public override async UniTask UseAbility(TickRate tickRate, CancellationToken ct)
        {
            BigNumber damageDeal = Owner.AttackDamage(out bool isCritical) * DamageScale;
            float attackSpeed = Owner.AttackSpeed;

            for (int i = 0; i < EnemiesCount; i++)
            {
                Enemies[i].WillTakeDamage(EnemiesTargetId[i], damageDeal);
            }
            Owner.SetFacingPosition(EnemyTarget.Transform.position);
            Owner.HeroAnim.PlayAttackAnimation(attackSpeed);
            await DelaySample(DelayFrame, tickRate, ct);
            VisualEffect.Spawn(EnemyTarget.Transform.position, Quaternion.identity).Play();
            await DelaySample(3, tickRate, ct);
            for (int i = 0; i < EnemiesCount; i++)
            {
                Enemies[i].TakeDamage(EnemiesTargetId[i], damageDeal, DamageType ,isCritical);
                Enemies[i].AddStatusEffect(new IceSlowStatusEffect(50, 3));
            }
            await DelaySample(30 - DelayFrame -3, tickRate, ct);
        }
    }
}