using System.Threading;
using Core.SimplePool;
using Cysharp.Threading.Tasks;
using TW.ACacheEverything;
using TW.Utility.CustomType;
using UnityEngine;

namespace Core
{
    [System.Serializable]
    public abstract partial class RangeAttackAbility : NormalAttackAbility
    {
        [field: SerializeField] public Projectile Projectile {get; private set;}
        protected Transform SpawnPosition {get; set;}
        
        public override async UniTask UseAbility(TickRate tickRate, CancellationToken ct)
        {
            BigNumber attackDamage = Owner.AttackDamage(out bool isCritical);
            float attackSpeed = Owner.AttackSpeed;

            if (!EnemyTarget.WillTakeDamage(EnemyTargetId,attackDamage, DamageType, out BigNumber finalDamage)) return;
            FinalDamage = finalDamage;
            Owner.SetFacingPosition(EnemyTarget.Transform.position);
            Owner.HeroAnim.PlayAttackAnimation(attackSpeed);
            await DelaySample(DelayFrame, tickRate, ct);
            Projectile.Spawn(SpawnPosition.position, Quaternion.identity)
                .Setup(Owner, EnemyTarget, EnemyTargetId, FinalDamage, DamageType, isCritical)
                .WithComplete(OnProjectileMoveCompleteCache);
            OnAttackComplete();
            await DelaySample(30 - DelayFrame, tickRate, ct);
        }
        [ACacheMethod("TW.Utility.CustomType")]
        private void OnProjectileMoveComplete(Hero ownerHero, Enemy targetEnemy, int targetEnemyId, BigNumber damage, DamageType damageType, bool isCritical)
        {
            if (!targetEnemy.TakeDamage(targetEnemyId, damage, damageType, isCritical)) return;
        }
    }
}