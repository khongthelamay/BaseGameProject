using System;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "PassiveAbility", menuName = "ScriptableObjects/PassiveAbility")]
    public class PassiveAbility : Ability
    {
        [field: SerializeReference] public PassiveAbilityBehavior PassiveAbilityBehavior {get; set;}

        public override bool CanUseAbility(Hero hero)
        {
            return PassiveAbilityBehavior.CanUseAbility(hero, AbilityTrigger, AbilityTarget);
        }

        public override bool TryUseAbility(Hero hero)
        {
            return PassiveAbilityBehavior.TryUseAbility(hero, AbilityTrigger, AbilityTarget);
        }
    }
    

}