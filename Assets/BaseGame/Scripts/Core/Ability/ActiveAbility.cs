using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core
{
    public abstract class ActiveAbility : Ability
    {
        protected ActiveAbility(Hero owner, int levelUnlock) : base(owner, levelUnlock)
        {

        }
        
        public abstract void OnEnterBattleField();
        public abstract void OnExitBattleField();
        public abstract bool CanUseAbility();
        public abstract UniTask UseAbility(TickRate tickRate, CancellationToken ct);
        public UniTask DelaySample(int sample, TickRate tickRate, CancellationToken ct)
        {
            int timeDelay = (int)(1000 / tickRate.ToValue() / Owner.AttackSpeed * sample / 30);
            return UniTask.WhenAny(UniTask.Delay(timeDelay, cancellationToken: ct),
                UniTask.WaitUntil(Owner.IsCancelAbilityMethodCache, cancellationToken: ct));
        }
    }
}