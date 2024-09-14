using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.DesignPattern;

namespace Core
{
    public class HeroMoveState : SingletonState<Hero, HeroMoveState>
    {
        
    }
    
    public partial class Hero : HeroMoveState.IHandler
    {
        public UniTask OnStateEnter(HeroMoveState state, CancellationToken token)
        {
            return UniTask.CompletedTask;
        }

        public UniTask OnStateExecute(HeroMoveState state, CancellationToken token)
        {
            return UniTask.CompletedTask;
        }

        public UniTask OnStateExit(HeroMoveState state, CancellationToken token)
        {
            return UniTask.CompletedTask;
        }
    }
}