using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.DesignPattern;

namespace Core
{
    public class HeroIdleState : IState
    {
        public interface IHandler
        {
            UniTask OnEnter(HeroIdleState state, CancellationToken ct);
            UniTask OnUpdate(HeroIdleState state, CancellationToken ct);
            UniTask OnExit(HeroIdleState state, CancellationToken ct);
        }
        private IHandler Owner { get; set; }

        public HeroIdleState(IHandler owner)
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
    public partial class Hero : HeroIdleState.IHandler
    {
        private HeroIdleState HeroIdleStateCache { get; set; }
        public HeroIdleState HeroIdleState => HeroIdleStateCache ??= new HeroIdleState(this);

        public UniTask OnEnter(HeroIdleState state, CancellationToken ct)
        {
            return UniTask.CompletedTask;
        }

        public UniTask OnUpdate(HeroIdleState state, CancellationToken ct)
        {
            return UniTask.CompletedTask;
        }

        public UniTask OnExit(HeroIdleState state, CancellationToken ct)
        {
            return UniTask.CompletedTask;
        }
    }
}