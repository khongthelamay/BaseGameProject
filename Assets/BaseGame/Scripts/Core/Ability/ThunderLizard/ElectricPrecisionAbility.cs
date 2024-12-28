using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "ElectricPrecisionAbility", menuName = "Ability/ThunderLizard/ElectricPrecisionAbility")]
    public class ElectricPrecisionAbility : PassiveAbility
    {
        [field: SerializeField] public int AttackCount {get; private set;}
        private ThunderLizard OwnerThunderLizard { get; set; }
        public override Ability WithOwnerHero(Hero owner)
        {
            OwnerThunderLizard = owner as ThunderLizard;
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
            OwnerThunderLizard.ForceCriticalCount++;
        }
    }
}