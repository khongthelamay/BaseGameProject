using UnityEngine;

namespace Core.GameStatusEffect
{
    [System.Serializable]
    public class AttackDamageChangeEffect : StatusEffect
    {
        [field: SerializeField] public float AttackDamageBuff { get; private set; }
        [field: SerializeField] public float Duration { get; private set; }

        public AttackDamageChangeEffect(float attackDamageBuff, float duration) : base(Type.AttackDamageChange, true)
        {
            AttackDamageBuff = attackDamageBuff;
            Duration = duration;
        }

        public override void OnAdd(IStatusEffectAble statusEffectAble)
        {
            base.OnAdd(statusEffectAble);
            if (statusEffectAble is IAttackDamageChangeAble attackDamageBuffAble)
            {
                attackDamageBuffAble.AttackDamageChange += AttackDamageBuff;
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
                attackDamageBuffAble.AttackDamageChange -= AttackDamageBuff;
            }
        }
    }
}