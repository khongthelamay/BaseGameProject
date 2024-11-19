using System.Threading;
using Core.SimplePool;
using Cysharp.Threading.Tasks;
using TW.ACacheEverything;
using TW.Utility.CustomType;
using UnityEngine;

namespace Core
{
    public partial class OrbCatalystAbility : PassiveAbility
    {
        private Archmage OwnerArchmage { get; set; }
        public OrbCatalystAbility(Hero owner, int levelUnlock) : base(owner, levelUnlock)
        {
            OwnerArchmage = (Archmage) owner;
        }

        public override void OnEnterBattleField()
        {
            OwnerArchmage.AddOrbSpawnRate(10);
        }

        public override void OnExitBattleField()
        {
            OwnerArchmage.AddOrbSpawnRate(-10);
        }
    }
}