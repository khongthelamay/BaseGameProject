using System.Threading;
using Core.SimplePool;
using Cysharp.Threading.Tasks;
using TW.ACacheEverything;
using TW.Utility.CustomType;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "RallyingFuryAbility", menuName = "Ability/Boar/RallyingFuryAbility")]
    public partial class RallyingFuryAbility : ActiveAbility, IAbilityTargetAnEnemy
    {
        [field: SerializeField] public int RallyingFuryRate {get; private set;}
        [field: SerializeField] public DamageType DamageType {get; private set;}
        [field: SerializeField] public int DelayFrame {get; private set;}
        [field: SerializeField] public float DamageScale {get; private set;}
        [field: SerializeField] public Projectile Projectile {get; private set;}
        [field: SerializeField] public VisualEffect VisualEffect {get; private set;}

        private Transform SpawnPosition {get; set;}
        public Enemy EnemyTarget { get; set; }
        public int EnemyTargetId { get; set; }
        private Boar OwnerBoar { get; set; }

        public override Ability WithOwnerHero(Hero owner)
        {
            OwnerBoar = owner as Boar;
            SpawnPosition = OwnerBoar?.ProjectileSpawnPosition;

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
            if (Random.Range(0, 100) >= RallyingFuryRate) return false;
            return this.IsFindAnyEnemyTarget();
        }

        public override async UniTask UseAbility(TickRate tickRate, CancellationToken ct)
        {
            BigNumber attackDamage = Owner.AttackDamage(out bool isCritical) * DamageScale;
            float attackSpeed = Owner.AttackSpeed;

            Owner.SetFacingPosition(EnemyTarget.Transform.position);
            Owner.HeroAnim.PlaySkill1Animation(attackSpeed);

            await DelaySample(DelayFrame, tickRate, ct);
            Projectile.Spawn(SpawnPosition.position, Quaternion.identity)
                .Setup(Owner, EnemyTarget, EnemyTargetId, attackDamage, DamageType, isCritical)
                .WithComplete(OnProjectileMoveCompleteCache);
            await DelaySample(30 - DelayFrame, tickRate, ct);
        }
        [ACacheMethod("TW.Utility.CustomType")]
        private void OnProjectileMoveComplete(Hero ownerHero, Enemy targetEnemy, int targetEnemyId, BigNumber damage, DamageType damageType, bool isCritical)
        {
            if (!targetEnemy.TakeDamage(targetEnemyId, damage, damageType, isCritical)) return;
            VisualEffect.Spawn(targetEnemy.Transform.position, Quaternion.identity)
                .Play();
        }


    }
}