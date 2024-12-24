using UnityEngine;

namespace Core.GameStatusEffect
{
    [System.Serializable]
    public class AttackDamageChangeEffect : StatusEffect
    {
        [field: SerializeField] public float AttackDamageChange { get; private set; }
        [field: SerializeField] public float Duration { get; private set; }

        public AttackDamageChangeEffect(float attackDamageChange, float duration) : base(Type.AttackDamageChange, true)
        {
            AttackDamageChange = attackDamageChange;
            Duration = duration;
        }

        public override void OnAdd(IStatusEffectAble statusEffectAble)
        {
            base.OnAdd(statusEffectAble);
            if (statusEffectAble is IAttackDamageChangeAble attackDamageChangeAble)
            {
                attackDamageChangeAble.AttackDamageChange += AttackDamageChange;
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
            if (statusEffectAble is IAttackDamageChangeAble attackDamageBuffAble)
            {
                attackDamageBuffAble.AttackDamageChange -= AttackDamageChange;
            }
        }
    }
}