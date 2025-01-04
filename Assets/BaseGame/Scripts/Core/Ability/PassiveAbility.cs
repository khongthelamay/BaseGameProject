namespace Core
{
    public abstract class PassiveAbility : Ability
    {
        protected override void SetupGroup()
        {
            base.SetupGroup();
            AbilityGroup |= Group.Passive;
        }
    }
}