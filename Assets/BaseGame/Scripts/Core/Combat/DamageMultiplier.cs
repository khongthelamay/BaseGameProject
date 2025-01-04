using UnityEngine;

namespace Core
{
    public static class DamageMultiplier
    {
        private static float ArmorFormulaScale { get; set; } = 1.2f;
        private static float ArmorFormulaBase { get; set; } = 1;
        private static float ArmorFormulaFactor { get; set; } = 0.06f;

        public static float GetDamageMultiplier(int armor)
        {
            return ArmorFormulaScale * (1 - ArmorFormulaFactor * armor / (ArmorFormulaBase + ArmorFormulaFactor * Mathf.Abs(armor)));
        }
        public static float GetDamageChangeMultiplier(int armor)
        {
            return GetDamageMultiplier(armor) - 1;
        }
    }
}