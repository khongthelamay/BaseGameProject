using TW.Utility.CustomType;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "StormSurgeAbility", menuName = "Ability/ThunderLizard/StormSurgeAbility")]
    public class StormSurgeAbility : PassiveAbility
    {
        [field: SerializeField] public float AttackDamageIncrease {get; private set;}
        [field: SerializeField] public int StackCount {get; private set;}
        private ThunderLizard OwnerThunderLizard { get; set; }
        public override Ability WithOwnerHero(Hero owner)
        {
            OwnerThunderLizard = owner as ThunderLizard;
            return base.WithOwnerHero(owner);
        }
        public override void OnEnterBattleField()
        {
            OwnerThunderLizard.AttackDamageChange += AttackDamageIncrease * StackCount;
        }

        public override void OnExitBattleField()
        {
            OwnerThunderLizard.AttackDamageChange -= AttackDamageIncrease * StackCount;
        }

        public override Ability ResetAbility()
        {
            StackCount = 0;
            return base.ResetAbility();
        }

        public void IncreaseStackCount()
        {
            StackCount += 1;
            OwnerThunderLizard.AttackDamageChange += AttackDamageIncrease;
        }
    }
}