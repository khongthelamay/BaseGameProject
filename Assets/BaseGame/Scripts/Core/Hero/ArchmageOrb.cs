using Core.SimplePool;
using TW.Utility.CustomComponent;
using UnityEngine;

namespace Core
{
    public class ArchmageOrb : ACachedMonoBehaviour, IPoolAble<ArchmageOrb>
    {
        [field: SerializeField] public Hero Hero {get; private set;}

        public ArchmageOrb Setup(Hero hero)
        {
            Hero = hero;
            return this;
        }
        public ArchmageOrb OnSpawn()
        {
            return this;
        }

        public void OnDespawn()
        {
            
        }
    }
}