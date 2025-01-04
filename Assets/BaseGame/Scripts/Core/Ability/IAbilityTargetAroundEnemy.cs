using Manager;

namespace Core
{
    public interface IAbilityTargetAroundEnemy
    {
        public Hero Owner { get; }
        public BattleManager BattleManager { get; }
        public float Radius { get; }
        public Enemy EnemyTarget { get; set; }
        public Enemy[] Enemies { get; set; }
        public int[] EnemiesTargetId { get; set; }
        public int EnemiesCount { get; set; }
    }
    
    public static class AbilityTargetAroundEnemyExtenstion
    {
        public static bool IsFindAnyEnemyTarget(this IAbilityTargetAroundEnemy abilityTargetAroundEnemy)
        {
            if (!abilityTargetAroundEnemy.BattleManager.TryGetEnemyInAttackRange(abilityTargetAroundEnemy.Owner, out Enemy target)) return false;
            abilityTargetAroundEnemy.EnemyTarget = target;
            abilityTargetAroundEnemy.EnemiesCount = abilityTargetAroundEnemy.BattleManager.GetEnemyAroundNonAlloc(
                abilityTargetAroundEnemy.EnemyTarget.Transform.position, 
                abilityTargetAroundEnemy.Radius, 
                abilityTargetAroundEnemy.Enemies);
            abilityTargetAroundEnemy.EnemiesTargetId = new int[abilityTargetAroundEnemy.EnemiesCount];
            for (int i = 0; i < abilityTargetAroundEnemy.EnemiesCount; i++)
            {
                abilityTargetAroundEnemy.EnemiesTargetId[i] = abilityTargetAroundEnemy.Enemies[i].Id;
            }
            return true;
        }
    }
}