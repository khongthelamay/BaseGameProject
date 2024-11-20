using Core;
using UnityEngine;

namespace Game.Core
{
    public class AnimatorEffect : VisualEffect
    {
        [field: SerializeField] public Animator Animator {get; private set;}
        [field: SerializeField] public string AnimationName {get; private set;}
        public override VisualEffect OnSpawn()
        {
            Animator.Play(AnimationName);
            return base.OnSpawn();
        }
    }
}