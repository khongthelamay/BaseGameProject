using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "SavageBlastAbility", menuName = "Ability/Boar/SavageBlastAbility")]
    public class SavageBlastAbility : ActiveCooldownAbility
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