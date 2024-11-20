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
            return Owner.OnEnter(this, ct);
        }

        public UniTask OnUpdate(CancellationToken ct)
        {
            return Owner.OnUpdate(this, ct);
        }

        public UniTask OnExit(CancellationToken ct)
        {
            return Owner.OnExit(this, ct);
        }
    }

    public partial class Hero : HeroIdleState.IHandler
    {
        private HeroIdleState HeroIdleStateCache { get; set; }
        public HeroIdleState HeroIdleState => HeroIdleStateCache ??= new HeroIdleState(this);

        public UniTask OnEnter(HeroIdleState state, CancellationToken ct)
        {
            HeroAnim.PlayIdleAnimation(1);
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