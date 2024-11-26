using UnityEngine;

namespace Core.TigerAbility
{
    [System.Serializable]
    public class FuryMasteryAbility : PassiveAbility
    {
        [field: SerializeField] public int FuryRate {get; private set;} = 10;

        private Tiger OwnerTiger { get; set; }
        public FuryMasteryAbility() 
        {
            
        }
        public FuryMasteryAbility(Hero owner) : base(owner)
        {
            OwnerTiger = owner as Tiger;
        }
        
        public override void OnEnterBattleField()
        {
            OwnerTiger.ChangeFuryRate(FuryRate);
        }
    
        public override void OnExitBattleField()
        {
            OwnerTiger.ChangeFuryRate(-FuryRate);
        }

        public override Ability Clone(Hero owner)
        {
            return new FuryMasteryAbility(owner)
            {
                LevelUnlock = LevelUnlock,
                Description = Description,
                FuryRate = FuryRate,
            };
        }
    }
}