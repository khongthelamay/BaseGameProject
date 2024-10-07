using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.DesignPattern;

namespace Core
{
    public class HeroIdleInShopState : SingletonState<Hero, HeroIdleInShopState>
    {
        
    }
    
    public partial class Hero : HeroIdleInShopState.IHandler
    {
        public UniTask OnStateEnter(HeroIdleInShopState state, CancellationToken token)
        {
            HeroAnim.PlayIdleAnimation(1);
            return UniTask.CompletedTask;
        }

        public UniTask OnStateExecute(HeroIdleInShopState state, CancellationToken token)
        {
            return UniTask.CompletedTask;
        }

        public UniTask OnStateExit(HeroIdleInShopState state, CancellationToken token)
        {
            return UniTask.CompletedTask;
        }
    }
}