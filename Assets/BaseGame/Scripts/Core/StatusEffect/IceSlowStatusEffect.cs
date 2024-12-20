using Core;
using UnityEngine;

[System.Serializable]
public class IceSlowStatusEffect : StatusEffect
{
    [field: SerializeField] public Color SlowColor {get; private set;} = new Color(0f, 0.5f, 1f);
    [field: SerializeField] public Color NormalColor {get; private set;} = Color.white;
    [field: SerializeField] public float SlowAmount {get; private set;}
    [field: SerializeField] public float Duration {get; private set;}
    public IceSlowStatusEffect(float slowAmount, float duration) : base(Type.Slow, true)
    {
        SlowAmount = slowAmount;
        Duration = duration;
    }

    public override void OnAdd(IStatusEffectAble statusEffectAble)
    {
        base.OnAdd(statusEffectAble);
        if (statusEffectAble is Enemy enemy)
        {
            enemy.SlowAmount.Value += SlowAmount;
            enemy.ChangeColor(SlowColor);
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
        if (statusEffectAble is Enemy enemy)
        {
            enemy.SlowAmount.Value -= SlowAmount;
            if (enemy.SlowAmount.Value <= 0.1f)
            {
                enemy.ChangeColor(NormalColor);
            }
        }
    }
}