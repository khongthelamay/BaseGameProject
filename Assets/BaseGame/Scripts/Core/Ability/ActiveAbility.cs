using System.Threading;
using Cysharp.Threading.Tasks;

namespace Core
{
    [System.Serializable]
    public abstract class ActiveAbility : Ability
    {
        public abstract bool CanUseAbility();
        public abstract UniTask UseAbility(TickRate tickRate, CancellationToken ct);
        private UniTask[] Tasks { get; set; } = new UniTask[2];
        protected UniTask DelaySample(int sample, TickRate tickRate, CancellationToken ct)
        {
            int timeDelay = (int)(1000 / tickRate.ToValue() / Owner.AttackSpeed * sample / 30);
            Tasks[0] = UniTask.Delay(timeDelay, cancellationToken: ct);
            Tasks[1] = UniTask.WaitUntil(Owner.IsCancelAbilityMethodCache, cancellationToken: ct);
            return UniTask.WhenAny(Tasks);
        }
    }
}