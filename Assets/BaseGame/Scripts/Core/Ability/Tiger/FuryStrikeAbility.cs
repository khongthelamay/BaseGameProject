using UnityEngine;

namespace Core.TigerAbility
{
    [System.Serializable]
    public class FuryStrikeAbility : PassiveAbility
    {
        [field: SerializeField] public int FuryRate {get; private set;} = 5;

        private Tiger OwnerTiger { get; set; }

        public FuryStrikeAbility()
        {
            
        }
        public FuryStrikeAbility(Hero owner) : base(owner)
        {
            OwnerTiger = (Tiger) owner;
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
            return new FuryStrikeAbility(owner)
            {
                LevelUnlock = LevelUnlock,
                Description = Description,
            };
        }
    }
}