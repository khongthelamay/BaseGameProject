using System;

namespace Core
{
    [Serializable]
    public abstract class PassiveAbilityBehavior 
    {
        public abstract bool CanUseAbility(Hero hero, Ability.Trigger trigger, Ability.Target target);
        public abstract bool TryUseAbility(Hero hero, Ability.Trigger trigger, Ability.Target target);
    }
}