using UnityEngine;

namespace Core.GameStatusEffect
{
    [System.Serializable]

    public class SlowStatusEffect : StatusEffect
    {
        [field: SerializeField] public float SlowAmount { get; private set; }
        [field: SerializeField] public float Duration { get; private set; }

        public SlowStatusEffect(float slowAmount, float duration) : base(Type.Slow, true)
        {
            SlowAmount = slowAmount;
            Duration = duration;
        }

        public override void OnAdd(IStatusEffectAble statusEffectAble)
        {
            base.OnAdd(statusEffectAble);
            if (statusEffectAble is ISlowAble slowAble)
            {
                slowAble.SlowAmount.Value += SlowAmount;
            }
        }

        public override void Execute(IStatusEffectAble statusEffectAble)
        {
            base.Execute(statusEffectAble);
            Duration -= Time.deltaTime;
            if (Duration <= 0)
            {
                statusEffectAble.RemoveStatusEffect(this);
            }
        }

        public override void OnRemove(IStatusEffectAble statusEffectAble)
        {
            base.OnRemove(statusEffectAble);
            if (statusEffectAble is ISlowAble slowAble)
            {
                slowAble.SlowAmount.Value -= SlowAmount;
            }
        }
    }
}