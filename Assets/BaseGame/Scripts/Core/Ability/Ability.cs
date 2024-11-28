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
        
        public virtual Ability WithOwnerHero(Hero owner)
        {
            Owner = owner;
            return this;
        }
        public virtual Ability ResetAbility()
        {
            return this;
        }
        public bool IsUnlocked(int level) => level >= LevelUnlock;
        
        public abstract void OnEnterBattleField();
        public abstract void OnExitBattleField();
        public Ability Create()
        {
            return Instantiate(this);
        }

        public Ability Initialize(Hero owner)
        {
            return Create().WithOwnerHero(owner);
        }
    }
}