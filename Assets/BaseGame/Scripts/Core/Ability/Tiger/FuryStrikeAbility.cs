using UnityEngine;

namespace Core.TigerAbility
{
    [System.Serializable]
    public class FuryStrikeAbility : PassiveAbility
    {
        [field: SerializeField] public int FuryRate {get; private set;} = 5;

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
        //     return ScriptableObject.CreateInstance<FuryStrikeAbility>();
        // }
    }
}