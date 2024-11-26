using System;
using Manager;
using TW.UGUI.Core.Activities;
using UnityEngine;

namespace Core
{
    [Serializable]
    public abstract class Ability : ScriptableObject
    {
        private BattleManager BattleManagerCache { get; set; }
        protected BattleManager BattleManager => BattleManagerCache ??= BattleManager.Instance;
        [field: SerializeField] public int LevelUnlock {get; set;}
        [field: SerializeField] public string Description {get; set;}
        public Hero Owner {get; private set;}

        protected Ability()
        {
            
        }
        protected Ability(Hero owner)
        {
            Owner = owner;
        }
        public bool IsUnlocked(int level) => level >= LevelUnlock;
        public abstract Ability Clone(Hero owner);
    }
}