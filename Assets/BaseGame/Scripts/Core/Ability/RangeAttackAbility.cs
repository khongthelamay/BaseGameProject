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
            BigNumber damageDeal = Owner.AttackDamage;
            float attackSpeed = Owner.AttackSpeed;
            
            EnemyTarget.WillTakeDamage(damageDeal);
            Owner.SetFacingPosition(EnemyTarget.Transform.position);
            Owner.HeroAnim.PlayAttackAnimation(attackSpeed);
            await DelaySample(DelayFrame, tickRate, ct);
            Projectile.Spawn(SpawnPosition.position, Quaternion.identity).Setup(Owner, EnemyTarget, damageDeal, DamageType)
                .WithComplete(OnProjectileMoveCompleteCache);
            OnAttackComplete();
            await DelaySample(30 - DelayFrame, tickRate, ct);
        }
        [ACacheMethod("TW.Utility.CustomType")]
        private void OnProjectileMoveComplete(Hero ownerHero, Enemy targetEnemy, BigNumber damage, DamageType damageType)
        {
            targetEnemy.TakeDamage(damage, damageType);
        }

        // public override void Clone(Ability ability)
        // {
        //     base.Clone(ability);
        //     if (ability is not RangeAttackAbility rangeAttackAbility) return;
        //     Projectile = rangeAttackAbility.Projectile;
        // }
    }
}