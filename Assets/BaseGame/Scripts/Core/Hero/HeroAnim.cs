using System.Threading;
using Cysharp.Threading.Tasks;
using Spine.Unity;
using UnityEngine;

namespace Core
{
    public class HeroAnim : MonoBehaviour
    {
        public enum State
        {
            None = 0,
            Idle = 1,
            Attack = 2,
        }

        private static readonly string Idle = "Idle";
        private static readonly string Attack = "Attack";

        [field: SerializeField] public SkeletonAnimation SkeletonAnimation { get; private set; }
        [field: SerializeField] public State CurrentState { get; private set; }
        private CancellationTokenSource CancellationTokenSource { get; set; }
        public HeroAnim PlayIdleAnimation(float speed)
        {
            if (CurrentState == State.Idle) return this;
            CurrentState = State.Idle;
            PlayAnimation(Idle, speed, true).Forget();
            return this;
        }

        public HeroAnim PlayAttackAnimation(float speed)
        {
            if (CurrentState == State.Attack) return this;
            CurrentState = State.Attack;
            PlayAnimation(Attack, speed, false).Forget();
            
            return this;
        }

        private async UniTask PlayAnimation(string animationName, float speed, bool loop = false)
        {
            if (SkeletonAnimation == null) return;
            CancellationTokenSource?.Cancel();
            CancellationTokenSource?.Dispose();
            CancellationTokenSource = null;
            
            
            
            SkeletonAnimation.timeScale = speed;
            SkeletonAnimation.AnimationState.SetAnimation(0, animationName, loop);

            if (loop) return;
            if (CurrentState == State.Idle) return;
            
            CancellationTokenSource = new CancellationTokenSource();
            await UniTask.Delay((int)(900 / speed), cancellationToken: CancellationTokenSource.Token);
            PlayIdleAnimation(1);
        }
    }
}