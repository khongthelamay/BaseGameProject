using System.Threading;
using Core.SimplePool;
using Cysharp.Threading.Tasks;
using Manager;
using TW.Utility.CustomType;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "DevastatingStrikeAbility", menuName = "Ability/Footman/DevastatingStrikeAbility")]
    public class DevastatingStrikeAbility : ActiveAbility
    {
        public override bool CanUseAbility()
        {
            return false;
        }
    
        public override void OnEnterBattleField()
        {
            
        }
    
        public override void OnExitBattleField()
        {
            
        }
        
        public override UniTask UseAbility(TickRate tickRate, CancellationToken ct)
        {
            return UniTask.CompletedTask;
        }
    }
}