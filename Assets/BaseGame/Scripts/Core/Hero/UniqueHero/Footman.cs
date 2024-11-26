using UnityEngine;

namespace Core
{
    public class Footman : Hero
    {
        [field: SerializeField] private VisualEffect DevastatingStrikeEffect {get; set;}
        protected override void InitAbility()
        {
            // ActiveAbilities.Add(new DevastatingStrikeAbility(this, DevastatingStrikeEffect));
            // ActiveAbilities.Add(new FootmanNormalAttack(this));
            
        }
    }
}