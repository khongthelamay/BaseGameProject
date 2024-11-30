using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "ArmorBreakAbility", menuName = "Ability/Knight/ArmorBreakAbility")]
    public class ArmorBreakAbility : ActiveAbility
    {
        public override void OnEnterBattleField()
        {
        }

        public override void OnExitBattleField()
        {
        }

        public override bool CanUseAbility()
        {
            return false;
        }

        public override UniTask UseAbility(TickRate tickRate, CancellationToken ct)
        {
            return UniTask.CompletedTask;
        }
        
    }
}