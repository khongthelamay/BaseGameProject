using UnityEngine;

namespace Core.TigerAbility
{
    [CreateAssetMenu(fileName = "FuryMasteryAbility", menuName = "Ability/Tiger/FuryMasteryAbility")]
    public class FuryMasteryAbility : PassiveAbility
    {
        [field: SerializeField] public int FuryStrikeRateBonus {get; private set;}
        
        public override void OnEnterBattleField()
        {
            if (Owner.TryGetAbility(out FuryStrikeAbility furyStrikeAbility))
            {
                furyStrikeAbility.ChangeFuryStrikeRate(FuryStrikeRateBonus);
            }
        }
    
        public override void OnExitBattleField()
        {
            if (Owner.TryGetAbility(out FuryStrikeAbility furyStrikeAbility))
            {
                furyStrikeAbility.ChangeFuryStrikeRate(-FuryStrikeRateBonus);
            }
        }
    }
}