using System.Threading;
using Cysharp.Threading.Tasks;

namespace Core
{
    [System.Serializable]
    public abstract class ActiveAbility : Ability
    {
        protected ActiveAbility()
        {
            
        }
        protected ActiveAbility(Hero owner) : base(owner)
        {

        }
        
        public abstract void OnEnterBattleField();
        public abstract void OnExitBattleField();
        public abstract bool CanUseAbility();
        public abstract UniTask UseAbility(TickRate tickRate, CancellationToken ct);

        protected UniTask DelaySample(int sample, TickRate tickRate, CancellationToken ct)
        {
            int timeDelay = (int)(1000 / tickRate.ToValue() / Owner.AttackSpeed * sample / 30);
            return UniTask.WhenAny(UniTask.Delay(timeDelay, cancellationToken: ct),
                UniTask.WaitUntil(Owner.IsCancelAbilityMethodCache, cancellationToken: ct));
        }
    }
}