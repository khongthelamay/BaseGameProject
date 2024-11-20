using System;
using Manager;
using UnityEngine;

namespace Core
{
    [Serializable]
    public abstract class Ability
    {
        private BattleManager BattleManagerCache { get; set; }
        protected BattleManager BattleManager => BattleManagerCache ??= BattleManager.Instance;
        [field: SerializeField] public int LevelUnlock {get; private set;}
        [field: SerializeField] public Hero Owner {get; private set;}

        protected Ability()
        {
            
        }
        protected Ability(Hero owner, int levelUnlock)
        {
            Owner = owner;
            LevelUnlock = levelUnlock;
        }
        public bool IsUnlocked(int level) => level >= LevelUnlock;

    }
}