using UnityEngine;

namespace Core
{
    public class FuryMasteryAbility : PassiveAbility
    {
        [field: SerializeField] public int FuryStrikeRateIncrease {get; private set;}

        private Tiger OwnerTiger { get; set; }
        public FuryMasteryAbility(Hero owner, int levelUnlock, int furyStrikeRateIncrease) : base(owner, levelUnlock)
        {
            FuryStrikeRateIncrease = furyStrikeRateIncrease;
            OwnerTiger = (Tiger) owner;
            
        }
        
        public override void OnEnterBattleField()
        {
            OwnerTiger.ChangeBonusFuryStrikeRate(FuryStrikeRateIncrease);
        }

        public override void OnExitBattleField()
        {
            OwnerTiger.ChangeBonusFuryStrikeRate(0);
        }
    }
}