using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "FrostMasteryAbility", menuName = "Ability/Mage/FrostMasteryAbility")]
    public class FrostMasteryAbility : PassiveAbility
    {
        [field: SerializeField] public float StunDurationBuff {get; private set;}

        private Mage OwnerMage { get; set; }
        public override Ability WithOwnerHero(Hero owner)
        {
            OwnerMage = owner as Mage;
            return base.WithOwnerHero(owner);
        }
        public override void OnEnterBattleField()
        {
            if (OwnerMage.TryGetAbility(out GlacialStrikeAbility glacialStrikeAbility))
            {
                glacialStrikeAbility.ChangeStunDuration(StunDurationBuff);
            }
            
        }

        public override void OnExitBattleField()
        {
            if (OwnerMage.TryGetAbility(out GlacialStrikeAbility glacialStrikeAbility))
            {
                glacialStrikeAbility.ChangeStunDuration(-StunDurationBuff);
            }
        }
    }
}