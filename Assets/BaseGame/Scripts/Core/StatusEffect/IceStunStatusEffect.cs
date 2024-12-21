using Core.SimplePool;
using UnityEngine;

namespace Core.GameStatusEffect
{
    [System.Serializable]
    public class IceStunStatusEffect : StunStatusEffect
    {
        [field: SerializeField] public VisualEffect StunEffect {get; private set;}

        public IceStunStatusEffect(float duration, VisualEffect stunEffect) : base(duration)
        {
            StunEffect = stunEffect;
        }

        public override void OnAdd(IStatusEffectAble statusEffectAble)
        {
            base.OnAdd(statusEffectAble);
            if (statusEffectAble is Enemy enemy)
            {
                StunEffect.Spawn(enemy.transform.position, Quaternion.identity, enemy.transform)
                    .WithLocalScale(1)
                    .Play();
            }
        }
    }
}