using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "ActiveAbility", menuName = "ScriptableObjects/ActiveAbility")]
    public class ActiveAbility : Ability
    {
        [field: SerializeField] public float Cooldown {get; private set;}
        [field: SerializeReference] public ActiveAbilityBehavior ActiveAbilityBehavior {get; private set;}

        public override bool CanUseAbility(Hero hero)       
        {
            return hero.GetCooldown(this) > Cooldown && ActiveAbilityBehavior.CanUseAbility(hero, Cooldown, AbilityTrigger, AbilityTarget);
        }

        public override bool TryUseAbility(Hero hero)
        {
            if (!ActiveAbilityBehavior.TryUseAbility(hero, Cooldown, AbilityTrigger, AbilityTarget)) return false;
            hero.SetCooldown(this, 0);
            return true;
        }
    }
}