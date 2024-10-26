using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.DesignPattern;

namespace Core
{
    public class HeroSleepState : IState
    {
        public interface IHandler
        {
            UniTask OnEnter(HeroSleepState state, CancellationToken ct);
            UniTask OnUpdate(HeroSleepState state, CancellationToken ct);
            UniTask OnExit(HeroSleepState state, CancellationToken ct);
        }
        private IHandler Owner { get; set; }

        public HeroSleepState(IHandler owner)
        {
            Owner = owner;
        }
        public UniTask OnEnter(CancellationToken ct)
        {
            Owner.OnEnter(this, ct);
            return UniTask.CompletedTask;   
        }

        public UniTask OnUpdate(CancellationToken ct)
        {
            Owner.OnUpdate(this, ct);
            return UniTask.CompletedTask;  
        }

        public UniTask OnExit(CancellationToken ct)
        {
            Owner.OnExit(this, ct);
            return UniTask.CompletedTask;  
        }
    }
    public partial class Hero : HeroSleepState.IHandler
    {
        private HeroSleepState HeroSleepStateCache { get; set; }
        public HeroSleepState HeroSleepState => HeroSleepStateCache ??= new HeroSleepState(this);

        public UniTask OnEnter(HeroSleepState state, CancellationToken ct)
        {
            return UniTask.CompletedTask;
        }

        public UniTask OnUpdate(HeroSleepState state, CancellationToken ct)
        {
            return UniTask.CompletedTask;
        }

        public UniTask OnExit(HeroSleepState state, CancellationToken ct)
        {
            return UniTask.CompletedTask;
        }
    }
}