using BaseGame.Scripts.Enum;
using Core.SimplePool;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using TW.Utility.CustomComponent;
using UnityEngine;

namespace Core
{
    public class Projectile : ACachedMonoBehaviour, IPoolAble<Projectile>
    {
        protected Hero OwnerHero { get; private set; }
        protected Enemy TargetEnemy { get; private set; }
        protected DamageType DamageType { get; private set; }
        public virtual Projectile Setup(Hero ownerHero, Enemy targetEnemy, DamageType damageType)
        {
            OwnerHero = ownerHero;
            TargetEnemy = targetEnemy;
            DamageType = damageType;
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