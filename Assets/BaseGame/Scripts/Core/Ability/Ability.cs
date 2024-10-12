using Sirenix.OdinInspector;
using UnityEngine;

namespace Core
{
    public abstract class Ability : ScriptableObject
    {
        public enum Trigger
        {
            NormalAttack,
            SpecialAttack,
            ProbabilityAttack,
            Active,
            Passive,
        }
        public enum Target      
        {
            Self,
            EnemyInRange,
            AllEnemiesInRange,
            AllyInRange,
            AllAlliesInRange,
        }
        [field: SerializeField] public Trigger AbilityTrigger {get; set;}
        [field: SerializeField] public Target AbilityTarget {get; set;}

        public abstract bool CanUseAbility(Hero hero);

        public abstract bool TryUseAbility(Hero hero);
    }
}