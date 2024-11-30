using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "BoarNormalAttack", menuName = "Ability/Boar/BoarNormalAttack")]
    public class BoarNormalAttack : RangeAttackAbility
    {
        private Boar OwnerBoar { get; set; }

        public override Ability WithOwnerHero(Hero owner)
        {
            OwnerBoar = owner as Boar;
            SpawnPosition = OwnerBoar.ProjectileSpawnPosition;

            return base.WithOwnerHero(owner);
        }
    }
}