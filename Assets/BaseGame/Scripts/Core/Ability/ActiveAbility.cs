using System.Threading;
using Cysharp.Threading.Tasks;

namespace Core
{
    public abstract class ActiveAbility : Ability
    {
        protected ActiveAbility(Hero owner, int levelUnlock) : base(owner, levelUnlock)
        {

        }
        
        public abstract bool CanUseAbility();
        public abstract UniTask UseAbility(TickRate tickRate, CancellationToken ct);
    }
}