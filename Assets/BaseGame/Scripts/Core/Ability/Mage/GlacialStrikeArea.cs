using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core
{
    public class GlacialStrikeArea : DamageOverTimeArea
    {
        private static readonly int Axis = Animator.StringToHash("Axis");
        [field: SerializeField] public Animator Animator {get; private set;}
        [field: SerializeField] public string AnimationName {get; private set;}

        public override UniTask StartDamageOverTime()
        {
            Animator.Play(AnimationName);
            return base.StartDamageOverTime();
        }

        public override void StopDamageOverTime()
        {
            base.StopDamageOverTime();
        }
        

        public override DamageOverTimeArea WithAxis(int axis)
        {
            Animator.SetFloat(Axis, axis);
            return base.WithAxis(axis);
        }
    }
}