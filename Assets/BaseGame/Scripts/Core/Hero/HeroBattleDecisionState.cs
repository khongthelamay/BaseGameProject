using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.DesignPattern;

namespace Core
{
    public class HeroBattleDecisionState : IState
    {
        public interface IHandler
        {
            UniTask OnEnter(HeroBattleDecisionState state, CancellationToken ct);
            UniTask OnUpdate(HeroBattleDecisionState state, CancellationToken ct);
            UniTask OnExit(HeroBattleDecisionState state, CancellationToken ct);
        }
        private IHandler Owner { get; set; }

        public HeroBattleDecisionState(IHandler owner)
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
    public partial class Hero : HeroBattleDecisionState.IHandler
    {
        private HeroBattleDecisionState HeroBattleDecisionStateCache { get; set; }
        public HeroBattleDecisionState HeroBattleDecisionState => HeroBattleDecisionStateCache ??= new HeroBattleDecisionState(this);

        public UniTask OnEnter(HeroBattleDecisionState state, CancellationToken ct)
        {
            return UniTask.CompletedTask;
        }

        public UniTask OnUpdate(HeroBattleDecisionState state, CancellationToken ct)
        {
            return UniTask.CompletedTask;
        }

        public UniTask OnExit(HeroBattleDecisionState state, CancellationToken ct)
        {
            return UniTask.CompletedTask;
        }
    }
}