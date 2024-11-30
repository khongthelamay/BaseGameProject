using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "JudgmentStrikeAbility", menuName = "Ability/Knight/JudgmentStrikeAbility")]
    public class JudgmentStrikeAbility : ActiveAbility
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