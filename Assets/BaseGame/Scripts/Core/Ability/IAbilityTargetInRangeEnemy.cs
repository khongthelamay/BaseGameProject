using Manager;
using UnityEngine;

namespace Core
{
    public interface IAbilityTargetInRangeEnemy
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
        public static bool IsFindAnyEnemyTarget(this IAbilityTargetInRangeEnemy abilityTargetInRangeEnemy)
        {
            if (!abilityTargetInRangeEnemy.BattleManager.TryGetEnemyInAttackRange(abilityTargetInRangeEnemy.Owner, out Enemy target)) return false;
            abilityTargetInRangeEnemy.EnemyTarget = target;
            abilityTargetInRangeEnemy.EnemyTargetId = target.Id;
            
            abilityTargetInRangeEnemy.EnemiesCount = abilityTargetInRangeEnemy.BattleManager.GetEnemyAroundNonAlloc(
                abilityTargetInRangeEnemy.EnemyTarget.Transform.position, 
                abilityTargetInRangeEnemy.Range.Current, 
                abilityTargetInRangeEnemy.Enemies);
            abilityTargetInRangeEnemy.EnemiesTargetId = new int[abilityTargetInRangeEnemy.EnemiesCount];
            for (int i = 0; i < abilityTargetInRangeEnemy.EnemiesCount; i++)
            {
                abilityTargetInRangeEnemy.EnemiesTargetId[i] = abilityTargetInRangeEnemy.Enemies[i].Id;
            }
            return true;
        }
    }
}