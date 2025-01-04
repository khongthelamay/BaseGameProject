using UnityEngine;

namespace Core.GameStatusEffect
{
    [System.Serializable]
    public class StunStatusEffect : StatusEffect
    {
        [field: SerializeField] public float Duration { get; private set; }
        
        public StunStatusEffect(float duration) : base(Type.Stun, false)
        {
            Duration = duration;
        }

        public override void OnAdd(IStatusEffectAble statusEffectAble)
        {
            base.OnAdd(statusEffectAble);
            if (statusEffectAble is IStunAble stunAble)
            {
                stunAble.IsStun.Value = true;
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
            if (statusEffectAble is IStunAble stunAble)
            {
                stunAble.IsStun.Value = false;
            }
        }
        
        public override void Overlap(IStatusEffectAble statusEffectAble, StatusEffect statusEffect)
        {
            base.Overlap(statusEffectAble, statusEffect);
            if (statusEffectAble is IStunAble stunAble)
            {
                stunAble.IsStun.Value = true;
                Duration = Mathf.Max(Duration, ((IceStunStatusEffect)statusEffect).Duration);
            }
        }
    }
}