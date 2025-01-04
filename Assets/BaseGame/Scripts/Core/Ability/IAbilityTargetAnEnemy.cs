using Manager;
using TW.Utility.CustomType;

namespace Core
{
    public interface IAbilityTargetAnEnemy
    {
        public Hero Owner { get; }
        public BattleManager BattleManager { get; }
        public Enemy EnemyTarget { get; set; }
        public int EnemyTargetId { get; set; }
    }

    public static class AbilityTargetAnEnemyExtenstion
    {
        public static bool IsFindAnyEnemyTarget(this IAbilityTargetAnEnemy abilityTargetAnEnemy)
        {
            if (!abilityTargetAnEnemy.BattleManager.TryGetEnemyInAttackRange(abilityTargetAnEnemy.Owner, out Enemy target)) return false;
            abilityTargetAnEnemy.EnemyTarget = target;
            abilityTargetAnEnemy.EnemyTargetId = target.Id;
            return true;
        }
        
        public static bool IsTargetStillAlive(this IAbilityTargetAnEnemy abilityTargetAnEnemy)
        {
            return abilityTargetAnEnemy.EnemyTarget != null && abilityTargetAnEnemy.EnemyTarget.Id == abilityTargetAnEnemy.EnemyTargetId && !abilityTargetAnEnemy.EnemyTarget.IsDead;
        }
        
        public static bool TryFindNewTarget(this IAbilityTargetAnEnemy abilityTargetAnEnemy)
        {
            return abilityTargetAnEnemy.IsFindAnyEnemyTarget();
        }
        
        public static bool ClearTarget(this IAbilityTargetAnEnemy abilityTargetAnEnemy)
        {
            abilityTargetAnEnemy.EnemyTarget = null;
            abilityTargetAnEnemy.EnemyTargetId = -1;
            return true;
        }
    }
}