using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "ArcherNormalAttack", menuName = "Ability/Archer/ArcherNormalAttack")]
    public class ArcherNormalAttack : RangeAttackAbility
    {
        private Archer OwnerArcher { get; set; }

        public override Ability WithOwnerHero(Hero owner)
        {
            OwnerArcher = owner as Archer;
            SpawnPosition = OwnerArcher?.ProjectileSpawnPosition;

            return base.WithOwnerHero(owner);
        }
    }
}