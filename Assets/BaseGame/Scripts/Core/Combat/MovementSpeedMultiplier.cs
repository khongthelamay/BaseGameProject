using UnityEngine;

namespace Core
{
    public static class MovementSpeedMultiplier
    {
        private static float MovementSpeedFormulaScale { get; set; } = 1f;
        private static float MovementSpeedFormulaBase { get; set; } = 6;
        private static float MovementSpeedFormulaFactor { get; set; } = 0.02f;

        public static float GetMovementSpeedMultiplier(float movementSpeedReduce)
        {
            return MovementSpeedFormulaScale * (1 - MovementSpeedFormulaFactor * movementSpeedReduce / (MovementSpeedFormulaBase + MovementSpeedFormulaFactor * Mathf.Abs(movementSpeedReduce)));
        }
    }
}