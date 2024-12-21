using Core;
using UnityEngine;

namespace Core.GameStatusEffect
{
    [System.Serializable]
    public class IceSlowStatusEffect : SlowStatusEffect
    {
        [field: SerializeField] public Color SlowColor { get; private set; } = new Color(0f, 0.5f, 1f);
        [field: SerializeField] public Color NormalColor { get; private set; } = Color.white;

        public IceSlowStatusEffect( float slowAmount, float duration) : base(slowAmount, duration)
        {
            
        }

        public override void OnAdd(IStatusEffectAble statusEffectAble)
        {
            base.OnAdd(statusEffectAble);
            if (statusEffectAble is Enemy enemy)
            {
                enemy.ChangeColor(SlowColor);
            }
        }

        public override void OnRemove(IStatusEffectAble statusEffectAble)
        {
            base.OnRemove(statusEffectAble);
            if (statusEffectAble is Enemy enemy)
            {
                if (enemy.SlowAmount.Value <= 0.1f)
                {
                    enemy.ChangeColor(NormalColor);
                }
            }
        }
    }
}