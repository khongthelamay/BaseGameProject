using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.CustomType;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "FootmanNormalAttack", menuName = "Ability/Footman/FootmanNormalAttack")]
    public class FootmanNormalAttack : MeleeAttackAbility
    {
        private Footman OwnerFootman { get; set; }

        public override Ability WithOwnerHero(Hero owner)
        {
            base.WithOwnerHero(owner);
            OwnerFootman = owner as Footman;
            return this;
        }
    }
}