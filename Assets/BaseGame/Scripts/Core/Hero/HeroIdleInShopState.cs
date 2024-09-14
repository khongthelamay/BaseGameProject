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
            throw new System.NotImplementedException();
        }

        public UniTask OnStateExecute(HeroIdleInShopState state, CancellationToken token)
        {
            throw new System.NotImplementedException();
        }

        public UniTask OnStateExit(HeroIdleInShopState state, CancellationToken token)
        {
            throw new System.NotImplementedException();
        }
    }
}