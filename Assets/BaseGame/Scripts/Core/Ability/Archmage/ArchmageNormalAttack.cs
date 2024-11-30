using System.Threading;
using Core.SimplePool;
using Cysharp.Threading.Tasks;
using TW.ACacheEverything;
using TW.Utility.CustomType;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "ArchmageNormalAttack", menuName = "Ability/Archmage/ArchmageNormalAttack")]
    public class ArchmageNormalAttack : RangeAttackAbility
    {
        private Archmage OwnerArchmage { get; set; }

        public override Ability WithOwnerHero(Hero owner)
        {
            base.WithOwnerHero(owner);
            OwnerArchmage = owner as Archmage;
            SpawnPosition = OwnerArchmage?.ProjectileSpawnPosition;
            return this;
        }
    }
}