using Core;
using Core.SimplePool;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Core
{
    public class AnimatorEffect : VisualEffect
    {
        private static readonly int Axis = Animator.StringToHash("Axis");
        [field: SerializeField, SuffixLabel("millisecond ", true)]
        public int SelfDespawnTime { get; protected set; }
        [field: SerializeField] public Animator Animator {get; private set;}
        [field: SerializeField] public string AnimationName {get; private set;}
        public override VisualEffect WithSpeed(float speed)
        {
            Animator.speed = speed;
            return base.WithSpeed(speed);
        }

        public override VisualEffect WithAxis(int axis)
        {
            Animator.SetFloat(Axis, axis);
            return base.WithAxis(axis);
        }

        public override VisualEffect Play()
        {
            Animator.Play(AnimationName);
            SelfDespawn().Forget();
            return base.Play();
        }
        private async UniTask SelfDespawn()
        {
            await UniTask.Delay(SelfDespawnTime, cancellationToken: this.GetCancellationTokenOnDestroy());
            this.Despawn();
        }
    }
}