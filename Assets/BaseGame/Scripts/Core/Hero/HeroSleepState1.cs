using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.DesignPattern;

namespace Core
{
    public class HeroSleepState1 : SingletonState<Hero, HeroSleepState1>
    {
        
    }
    public partial class Hero : HeroSleepState1.IHandler
    {
        public UniTask OnStateEnter(HeroSleepState1 state1, CancellationToken token)
        {
            return UniTask.CompletedTask;
        }

        public UniTask OnStateExecute(HeroSleepState1 state1, CancellationToken token)
        {
            return UniTask.CompletedTask;
        }

        public UniTask OnStateExit(HeroSleepState1 state1, CancellationToken token)
        {
            return UniTask.CompletedTask;
        }
    }
}