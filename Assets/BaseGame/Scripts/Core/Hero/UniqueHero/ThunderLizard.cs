using BaseGame.Scripts.Enum;

namespace Core
{
    public class ThunderLizard : Hero
    {
        protected override void InitAbility()
        {
            ActiveAbilities.Add(new MeleeAttackAbility(this, 0, DamageType.Physical));
        }
    }
}