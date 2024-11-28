using UnityEngine;

namespace Core.WolfAbility
{
    [CreateAssetMenu(fileName = "WolfNormalAttackAbility", menuName = "Ability/Wolf/WolfNormalAttackAbility")]
    public class WolfNormalAttackAbility : MeleeAttackAbility
    {
        private Wolf OwnerWolf { get; set; }

        public override Ability WithOwnerHero(Hero owner)
        {
            base.WithOwnerHero(owner);
            OwnerWolf = owner as Wolf;
            return this;
        }
    }
}