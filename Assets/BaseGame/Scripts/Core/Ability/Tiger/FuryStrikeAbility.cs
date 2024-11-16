using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.CustomType;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Core
{
    public class FuryStrikeAbility : PassiveAbility
    {
        private Tiger OwnerTiger { get; set; }
        public FuryStrikeAbility(Hero owner, int levelUnlock) : base(owner, levelUnlock)
        {
            OwnerTiger = (Tiger) owner;
        }
        
        public override void OnEnterBattleField()
        {
            OwnerTiger.AddFuryRate(10);
        }

        public override void OnExitBattleField()
        {
            OwnerTiger.AddFuryRate(-10);
        }
    }
}