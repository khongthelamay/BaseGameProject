using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "MageNormalAttack", menuName = "Ability/Mage/MageNormalAttack")]
    public class MageNormalAttack : RangeAttackAbility
    {
        private Mage OwnerMage { get; set; }

        public override Ability WithOwnerHero(Hero owner)
        {
            OwnerMage = owner as Mage;
            SpawnPosition = OwnerMage?.ProjectileSpawnPosition;

            return base.WithOwnerHero(owner);
        }
        
    }
}