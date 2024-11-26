using UnityEngine;

namespace Core.WolfAbility
{
    [System.Serializable]
    public class WolfNormalAttackAbility : MeleeAttackAbility
    {
        private Wolf OwnerWolf { get; set; }

        public WolfNormalAttackAbility()
        {
            
        }
        public WolfNormalAttackAbility(Hero owner) : base(owner)
        {
            OwnerWolf = owner as Wolf;
        }

        public override Ability Clone(Hero owner)
        {
            return new WolfNormalAttackAbility(owner)
            {
                DamageType = DamageType,
                DelayFrame = DelayFrame,
                Description = Description,
                LevelUnlock = LevelUnlock
            };
        }
    }
}