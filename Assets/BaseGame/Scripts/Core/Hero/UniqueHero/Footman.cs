using UnityEngine;

namespace Core
{
    public class Footman : Hero
    {
        [field: SerializeField] private VisualEffect DevastatingStrikeEffect {get; set;}
        protected override void InitAbility()
        {
            ActiveAbilities.Add(new DevastatingStrikeAbility(this, 0, DevastatingStrikeEffect));
            ActiveAbilities.Add(new FootmanNormalAttack(this, 0));
            
        }
    }
}