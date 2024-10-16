using System;

namespace Core
{
    [Serializable]
    public abstract class ActiveAbilityBehavior
    {
        public abstract bool CanUseAbility(Hero hero, float cooldown, Ability.Trigger trigger, Ability.Target target);
        public abstract bool TryUseAbility(Hero hero, float cooldown, Ability.Trigger trigger, Ability.Target target);
    }
}