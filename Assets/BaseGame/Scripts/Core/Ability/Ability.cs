using System;
using UnityEngine;

namespace Core
{
    [Serializable]
    public abstract class Ability
    {
        protected Hero Owner { get; set; }
        [field: SerializeField] public int LevelUnlock {get; private set;}

        protected Ability()
        {
            
        }
        protected Ability(Hero owner, int levelUnlock)
        {
            Owner = owner;
            LevelUnlock = levelUnlock;
        }
        public abstract Ability CreateAbility(Hero owner);
        public bool IsUnlocked(int level) => level >= LevelUnlock;

    }
}