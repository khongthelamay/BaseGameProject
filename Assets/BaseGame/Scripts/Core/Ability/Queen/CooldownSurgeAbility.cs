namespace Core
{
    public class CooldownSurgeAbility : PassiveAbility
    {
        private Queen OwnerQueen { get; set; }
        public CooldownSurgeAbility(Hero owner, int levelUnlock) : base(owner, levelUnlock)
        {
            OwnerQueen = owner as Queen;
        }

        public override void OnEnterBattleField()
        {
            OwnerQueen.ChangeCooldownSurgeRate(8);
            OwnerQueen.ChangeCooldownSurgeDecrease(10);
        }

        public override void OnExitBattleField()
        {
            OwnerQueen.ChangeCooldownSurgeRate(-8);
            OwnerQueen.ChangeCooldownSurgeDecrease(-10);
        }
    }
}