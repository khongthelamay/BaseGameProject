namespace Core
{
    public abstract class PassiveAbility : Ability
    {
        protected PassiveAbility()
        {
            
        }
        protected PassiveAbility(Hero owner) : base(owner)
        {
            
        }
        
        public abstract void OnEnterBattleField();
        public abstract void OnExitBattleField();
    }
}