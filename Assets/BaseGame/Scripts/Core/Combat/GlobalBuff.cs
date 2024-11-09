using UnityEngine;

namespace Core
{
    [System.Serializable]
    public class GlobalBuff
    {
        public enum Type
        {
            AttackDamage,
            AttackSpeed,
            CriticalChance,
            CriticalDamage,
            ArmorReduction,
            MagicResistanceReduction,
        }
        
        [field: SerializeField] public Type BuffType { get; private set; }
        [field: SerializeField] public float Value { get; private set; }

        public GlobalBuff(Type buffType, float value)
        {
            BuffType = buffType;
            Value = value;
        }
        public void ChangeValue(float value)
        {
            Value += value;
        }
    }
}