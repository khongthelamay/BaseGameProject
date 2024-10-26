using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.DesignPattern;
using UnityEngine;

namespace Core
{
    public class HeroMoveState1 : SingletonState<Hero, HeroMoveState1>
    {
        
    }
    
    public partial class Hero : HeroMoveState1.IHandler
    {
        public UniTask OnStateEnter(HeroMoveState1 state1, CancellationToken token)
        {
            // HeroAnim.PlayIdleAnimation(1);
            Instantiate(MoveProjectile, Transform.position, Quaternion.identity)
                .Init(MoveFromPosition, MoveToPosition, OnMoveComplete);
            
            return UniTask.CompletedTask;
        }

        public UniTask OnStateExecute(HeroMoveState1 state1, CancellationToken token)
        {
            return UniTask.CompletedTask;
        }

        public UniTask OnStateExit(HeroMoveState1 state1, CancellationToken token)
        {
            return UniTask.CompletedTask;
        }
        // private void OnMoveComplete()
        // {
        //     SetVisible(true);
        //     StateMachine.RequestTransition(HeroAttackState.Instance);
        // }
    }
}