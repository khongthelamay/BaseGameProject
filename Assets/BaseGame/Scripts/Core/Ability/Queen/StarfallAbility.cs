using System.Threading;
using Cysharp.Threading.Tasks;

namespace Core
{
    public class StarfallAbility : ActiveCooldownAbility
    {
        public StarfallAbility(Hero owner, int levelUnlock, float cooldown) : base(owner, levelUnlock, cooldown)
        {
            
        }

        public override void OnEnterBattleField()
        {
            
        }

        public override void OnExitBattleField()
        {
            
        }

        public override bool CanUseAbility()
        {
            return true;
        }

        public override UniTask UseAbility(TickRate tickRate, CancellationToken ct)
        {
            return UniTask.CompletedTask;
        }
    }
}