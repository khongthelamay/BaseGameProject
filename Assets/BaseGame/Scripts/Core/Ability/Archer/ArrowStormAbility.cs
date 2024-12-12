using System.Threading;
using Core.SimplePool;
using Cysharp.Threading.Tasks;
using LitMotion;
using TW.ACacheEverything;
using TW.Utility.CustomType;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "ArrowStormAbility", menuName = "Ability/Archer/ArrowStormAbility")]
    public partial class ArrowStormAbility : ActiveCooldownAbility, IAbilityTargetAnEnemy
    {
        [field: SerializeField] public int PiercingShotRate {get; private set;}
        [field: SerializeField] public DamageType DamageType {get; private set;}
        [field: SerializeField] public int DelayFrame {get; private set;}
        [field: SerializeField] public float DamageScale {get; private set;}

        [field: SerializeField] public Projectile Projectile {get; private set;}
        [field: SerializeField] public VisualEffect VisualEffect {get; private set;}

        private Transform SpawnPosition {get; set;}
        public Enemy EnemyTarget { get; set; }
        public int EnemyTargetId { get; set; }
        private Archer OwnerArcher { get; set; }

        public override Ability WithOwnerHero(Hero owner)
        {
            OwnerArcher = owner as Archer;
            SpawnPosition = OwnerArcher?.ProjectileSpawnPosition;

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
            if (Random.Range(0, 100) >= PiercingShotRate) return false;
            return this.IsFindAnyEnemyTarget();
        }

        public override async UniTask UseAbility(TickRate tickRate, CancellationToken ct)
        {
            BigNumber damageDeal = Owner.AttackDamage(out bool isCritical) * DamageScale;
            float attackSpeed = Owner.AttackSpeed;

            if (!EnemyTarget.WillTakeDamage(EnemyTargetId,damageDeal)) return;
            Owner.SetFacingPosition(EnemyTarget.Transform.position);
            Owner.HeroAnim.PlaySkill1Animation(attackSpeed);
            await DelaySample(DelayFrame, tickRate, ct);
            VisualEffect.Spawn(EnemyTarget.Transform.position, Quaternion.identity);
            // Projectile.Spawn(SpawnPosition.position, Quaternion.identity)
            //     .Setup(Owner, EnemyTarget, EnemyTargetId, damageDeal, DamageType, isCritical)
            //     .WithComplete(OnProjectileMoveCompleteCache);
            await DelaySample(30 - DelayFrame, tickRate, ct);
        }
        [ACacheMethod("TW.Utility.CustomType")]
        private void OnProjectileMoveComplete(Hero ownerHero, Enemy targetEnemy, int targetEnemyId, BigNumber damage, DamageType damageType, bool isCritical)
        {
            if (!targetEnemy.TakeDamage(targetEnemyId, damage, damageType, isCritical)) return;
        }


    }
}