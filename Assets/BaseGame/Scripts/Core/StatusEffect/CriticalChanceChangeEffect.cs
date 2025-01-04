using UnityEngine;

namespace Core.GameStatusEffect
{
    [System.Serializable]
    public class CriticalChanceChangeEffect : StatusEffect
    {
        [field: SerializeField] public int CriticalChanceChange {get; private set;}
        [field: SerializeField] public float Duration {get; private set;}
        public CriticalChanceChangeEffect(int criticalChanceChange, float duration) : base(Type.CriticalChanceChange, true)
        {
            CriticalChanceChange = criticalChanceChange;
            Duration = duration;
        }

        public override void OnAdd(IStatusEffectAble statusEffectAble)
        {
            base.OnAdd(statusEffectAble);
            if (statusEffectAble is ICriticalChanceChangeAble criticalChanceChangeAble)
            {
                criticalChanceChangeAble.CriticalChanceChange += CriticalChanceChange;
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
            if (statusEffectAble is ICriticalChanceChangeAble criticalChanceChangeAble)
            {
                criticalChanceChangeAble.CriticalChanceChange -= CriticalChanceChange;
            }
        }
    }
}