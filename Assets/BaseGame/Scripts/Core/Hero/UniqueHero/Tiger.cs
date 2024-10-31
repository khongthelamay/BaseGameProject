using BaseGame.Scripts.Enum;

namespace Core
{
    public class Tiger : Hero
    {
        protected override void InitAbility()
        {
            ActiveAbilities.Add(new MeleeAttackAbility(this, 0, DamageType.Physical));
        }
    }
}