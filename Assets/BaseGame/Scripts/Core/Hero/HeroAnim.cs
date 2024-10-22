using System;
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
        private static readonly int TickRate = Animator.StringToHash("TickRate");

        [field: SerializeField] public Animator Animator { get; private set; }

        [field: SerializeField] public State CurrentState { get; private set; }
        private CancellationTokenSource CancellationTokenSource { get; set; }


        public HeroAnim PlayIdleAnimation(float speed)
        {
            PlayAnimation(Idle, speed);
            return this;
        }

        public HeroAnim PlayAttackAnimation(float speed)
        {
            PlayAnimation(Attack, speed);

            return this;
        }

        private void PlayAnimation(string animationName, float speed)
        {
            if (Animator.runtimeAnimatorController == null) return;
            Animator.SetFloat(TickRate, speed);
            Animator.Play(animationName);
        }
    }
}