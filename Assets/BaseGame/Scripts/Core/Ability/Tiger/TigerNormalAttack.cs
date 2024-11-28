using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.CustomType;
using UnityEngine;

namespace Core.TigerAbility
{
    public class TigerNormalAttack : MeleeAttackAbility
    {
        private Tiger OwnerTiger { get; set; }

        public override Ability WithOwnerHero(Hero owner)
        {
            base.WithOwnerHero(owner);
            OwnerTiger = owner as Tiger;
            return this;
        }
    }
}