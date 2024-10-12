using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using TW.Utility.CustomComponent;
using UnityEngine;

namespace Core
{
    public class Projectile : ACachedMonoBehaviour
    {


        [field: SerializeField] protected ProjectileBehaviorGroup ProjectileBehaviorGroup { get; set; }
        
        public virtual Projectile Spawn(Hero hero, Enemy targetEnemy)
        {
            return this;
        }
        public virtual Projectile Setup(Hero hero, Enemy targetEnemy)
        {
            return this;
        }
        protected void SelfDespawn()
        {
            Destroy(gameObject);
        }
    }
}