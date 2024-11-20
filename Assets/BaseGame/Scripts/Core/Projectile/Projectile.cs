using System;
using Core.SimplePool;
using TW.Utility.CustomComponent;
using TW.Utility.CustomType;

namespace Core
{
    public class Projectile : ACachedMonoBehaviour, IPoolAble<Projectile>
    {
        protected Hero OwnerHero { get; private set; }
        protected Enemy TargetEnemy { get; private set; }
        protected BigNumber DamageDeal { get; private set; }
        protected DamageType DamageType { get; private set; }
        protected Action<Hero, Enemy, BigNumber, DamageType> OnCompleteCallback { get; private set; }
        public virtual Projectile Setup(Hero ownerHero, Enemy targetEnemy, BigNumber damage, DamageType damageType)
        {
            OwnerHero = ownerHero;
            TargetEnemy = targetEnemy;
            DamageDeal = damage;
            DamageType = damageType;
            return this;
        }

        public Projectile WithComplete(Action<Hero, Enemy, BigNumber, DamageType> onCompleteCallback)
        {
            OnCompleteCallback = onCompleteCallback;
            return this;
        }
        public virtual Projectile OnSpawn()
        {
            return this;
        }

        public virtual void OnDespawn()
        {
            
        }
    }
}