using UnityEngine;

namespace Core.GameStatusEffect
{
    [System.Serializable]
    public class CriticalDamageChangeEffect : StatusEffect
    {
        [field: SerializeField] public int CriticalChanceChange {get; private set;}
        [field: SerializeField] public float Duration {get; private set;}
        public CriticalDamageChangeEffect(int criticalChanceChange, float duration) : base(Type.CriticalDamageChange, true)
        {
            CriticalChanceChange = criticalChanceChange;
            Duration = duration;
        }

        public override void OnAdd(IStatusEffectAble statusEffectAble)
        {
            base.OnAdd(statusEffectAble);
            if (statusEffectAble is ICriticalDamageChangeAble criticalDamageChangeAble)
            {
                criticalDamageChangeAble.CriticalDamageChange += CriticalChanceChange;
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
            if (statusEffectAble is ICriticalDamageChangeAble criticalDamageChangeAble)
            {
                criticalDamageChangeAble.CriticalDamageChange -= CriticalChanceChange;
            }
        }
    }
}