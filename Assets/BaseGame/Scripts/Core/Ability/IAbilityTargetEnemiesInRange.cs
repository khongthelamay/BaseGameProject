using Manager;
using TW.Utility.CustomType;
using UnityEngine;

namespace Core
{
    public interface IAbilityTargetEnemiesInRange
    {
        public Hero Owner { get; }
        public BattleManager BattleManager { get; }
        public AxisDetection<Vector2> Range { get; set; }
        public Enemy EnemyTarget { get; set; }
        public int EnemyTargetId { get; set; }
        public Enemy[] Enemies { get; set; }
        public int[] EnemiesTargetId { get; set; }
        public int EnemiesCount { get; set; }
    }
    
    public static class AbilityTargetInRangeEnemyExtenstion
    {
        public static bool IsFindAnyEnemyTarget(this IAbilityTargetEnemiesInRange abilityTargetEnemiesInRange)
        {
            if (!abilityTargetEnemiesInRange.BattleManager.TryGetEnemyInAttackRange(abilityTargetEnemiesInRange.Owner, out Enemy target)) return false;
            abilityTargetEnemiesInRange.EnemyTarget = target;
            abilityTargetEnemiesInRange.EnemyTargetId = target.Id;
            
            abilityTargetEnemiesInRange.EnemiesCount = abilityTargetEnemiesInRange.BattleManager.GetEnemyAroundNonAlloc(
                abilityTargetEnemiesInRange.EnemyTarget.Transform.position, 
                abilityTargetEnemiesInRange.Range.Current, 
                abilityTargetEnemiesInRange.Enemies);
            abilityTargetEnemiesInRange.EnemiesTargetId = new int[abilityTargetEnemiesInRange.EnemiesCount];
            for (int i = 0; i < abilityTargetEnemiesInRange.EnemiesCount; i++)
            {
                abilityTargetEnemiesInRange.EnemiesTargetId[i] = abilityTargetEnemiesInRange.Enemies[i].Id;
            }
            return true;
        }
    }
}