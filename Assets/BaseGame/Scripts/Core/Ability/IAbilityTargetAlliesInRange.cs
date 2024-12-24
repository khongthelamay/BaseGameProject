using Manager;
using UnityEngine;

namespace Core
{
    public interface IAbilityTargetAlliesInRange
    {
        public Hero Owner { get; }
        public BattleManager BattleManager { get; }
        public AxisDetection<Vector2> AlliesRange { get; set; }
        public Hero[] Heroes { get; set; }
        public int[] HeroesId { get; set; }
        public int HeroesCount { get; set; }
    }
    
    public static class AbilityTargetInRangeAlliesExtenstion
    {
        public static bool IsFindAnyAlliesTarget(this IAbilityTargetAlliesInRange abilityTargetAlliesInRange)
        {
            abilityTargetAlliesInRange.HeroesCount = abilityTargetAlliesInRange.BattleManager.GetHeroAroundNonAlloc(
                abilityTargetAlliesInRange.Owner.Transform.position, 
                abilityTargetAlliesInRange.AlliesRange.Current, 
                abilityTargetAlliesInRange.Heroes);
            abilityTargetAlliesInRange.HeroesId = new int[abilityTargetAlliesInRange.HeroesCount];
            for (int i = 0; i < abilityTargetAlliesInRange.HeroesCount; i++)
            {
                abilityTargetAlliesInRange.HeroesId[i] = abilityTargetAlliesInRange.Heroes[i].Id;
            }
            return true;
        }
    }
}