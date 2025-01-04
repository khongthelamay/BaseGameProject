using UnityEngine;

namespace Core.GameStatusEffect
{
    [System.Serializable]
    public class PhysicalArmorChangeEffect : StatusEffect
    {
        [field: SerializeField] public int PhysicalArmorChange {get; private set;}
        [field: SerializeField] public float Duration {get; private set;}
        public PhysicalArmorChangeEffect(int physicalArmorChange, float duration) : base(Type.PhysicalArmorChange, true)
        {
            PhysicalArmorChange = physicalArmorChange;
            Duration = duration;
        }

        public override void OnAdd(IStatusEffectAble statusEffectAble)
        {
            base.OnAdd(statusEffectAble);
            if (statusEffectAble is IPhysicalArmorChangeAble physicalArmorChangeAble)
            {
                physicalArmorChangeAble.PhysicalArmorChange += PhysicalArmorChange;
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
            if (statusEffectAble is IPhysicalArmorChangeAble physicalArmorChangeAble)
            {
                physicalArmorChangeAble.PhysicalArmorChange -= PhysicalArmorChange;
            }
        }
    }
}