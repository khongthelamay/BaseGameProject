using Core;
using UnityEngine;

namespace Game.Core
{
    public class AnimatorEffect : VisualEffect
    {
        private static readonly int Axis = Animator.StringToHash("Axis");
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

        public override VisualEffect OnSpawn()
        {
            Animator.Play(AnimationName);
            return base.OnSpawn();
        }
    }
}