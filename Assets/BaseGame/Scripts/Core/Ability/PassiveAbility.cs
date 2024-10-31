namespace Core
{
    public abstract class PassiveAbility : Ability
    {
        protected PassiveAbility(Hero owner, int levelUnlock) : base(owner, levelUnlock)
        {
            
        }
        
        public abstract void OnEnterBattleField();
        public abstract void OnExitBattleField();
    }
}