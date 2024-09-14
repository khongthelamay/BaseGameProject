using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.DesignPattern;

namespace Core
{
    public class HeroSleepState : SingletonState<Hero, HeroSleepState>
    {
        
    }
    public partial class Hero : HeroSleepState.IHandler
    {
        public UniTask OnStateEnter(HeroSleepState state, CancellationToken token)
        {
            return UniTask.CompletedTask;
        }

        public UniTask OnStateExecute(HeroSleepState state, CancellationToken token)
        {
            return UniTask.CompletedTask;
        }

        public UniTask OnStateExit(HeroSleepState state, CancellationToken token)
        {
            return UniTask.CompletedTask;
        }
    }
}