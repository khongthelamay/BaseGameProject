using UnityEngine;

namespace Core.TigerAbility
{
    [System.Serializable]
    public class FuryMasteryAbility : PassiveAbility
    {
        [field: SerializeField] public int FuryRate {get; private set;} = 10;

        private Tiger OwnerTiger { get; set; }
        
        public override void OnEnterBattleField()
        {
            OwnerTiger.ChangeFuryRate(FuryRate);
        }
    
        public override void OnExitBattleField()
        {
            OwnerTiger.ChangeFuryRate(-FuryRate);
        }

        // public override Ability Create()
        // {
        //     return ScriptableObject.CreateInstance<FuryMasteryAbility>();
        // }
    }
}