
using System;
using UnityEngine;

[System.Serializable]
public class StatusEffect
{
    public enum Type
    {
        Slow,
        Stun,
    }
    [field: SerializeField] public Type StatusEffectType { get; private set; }
    [field: SerializeField] public bool IsStackable { get; private set; }
    
    public StatusEffect(Type statusEffectType, bool isStackable)
    {
        StatusEffectType = statusEffectType;
        IsStackable = isStackable;
    }

    public virtual void Overlap(IStatusEffectAble statusEffectAble, StatusEffect statusEffect)
    {
        
    }
    public virtual void OnAdd(IStatusEffectAble statusEffectAble)
    {
        
    }
    public virtual void Execute(IStatusEffectAble statusEffectAble)
    {
        
    }
    public virtual void OnRemove(IStatusEffectAble statusEffectAble)
    {
        
    }

}