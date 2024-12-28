using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.CustomType;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "RendingClawsAbility", menuName = "Ability/Bear/RendingClawsAbility")]
    public class RendingClawsAbility : PassiveAbility
    {
        [field: SerializeField] public int AttackCount {get; private set;}
        private Bear OwnerBear { get; set; }
        public override Ability WithOwnerHero(Hero owner)
        {
            OwnerBear = owner as Bear;
            return base.WithOwnerHero(owner);
        }

        public override void OnEnterBattleField()
        {
        }

        public override void OnExitBattleField()
        {
        }

        public override Ability ResetAbility()
        {
            AttackCount = 0;
            return base.ResetAbility();
        }

        public void AddAttackCount()
        {
            AttackCount++;
            if (AttackCount < 2) return;
            AttackCount -= 2;
            OwnerBear.ForceCriticalCount++;
        }


    }
}