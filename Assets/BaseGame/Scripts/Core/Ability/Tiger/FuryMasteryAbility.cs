using UnityEngine;

namespace Core
{
    public class FuryMasteryAbility : PassiveAbility
    {
        private Tiger OwnerTiger { get; set; }
        public FuryMasteryAbility(Hero owner, int levelUnlock) : base(owner, levelUnlock)
        {
            OwnerTiger = (Tiger) owner;
        }
        
        public override void OnEnterBattleField()
        {
            OwnerTiger.AddFuryRate(10);
        }

        public override void OnExitBattleField()
        {
            OwnerTiger.AddFuryRate(-10);
        }
    }
}