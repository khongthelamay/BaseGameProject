using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.DesignPattern;

namespace Core
{
    public class HeroAttackState : IState
    {
        public interface IHandler
        {
            UniTask OnEnter(HeroAttackState state, CancellationToken ct);
            UniTask OnUpdate(HeroAttackState state, CancellationToken ct);
            UniTask OnExit(HeroAttackState state, CancellationToken ct);
        }

        private IHandler Owner { get; set; }

        public HeroAttackState(IHandler owner)
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

    public partial class Hero : HeroAttackState.IHandler
    {
        private HeroAttackState HeroAttackStateCache { get; set; }
        public HeroAttackState HeroAttackState => HeroAttackStateCache ??= new HeroAttackState(this);

        public UniTask OnEnter(HeroAttackState state, CancellationToken ct)
        {
            return UniTask.CompletedTask;
        }

        public UniTask OnUpdate(HeroAttackState state, CancellationToken ct)
        {
            return UniTask.CompletedTask;
        }

        public UniTask OnExit(HeroAttackState state, CancellationToken ct)
        {
            return UniTask.CompletedTask;
        }
    }
}