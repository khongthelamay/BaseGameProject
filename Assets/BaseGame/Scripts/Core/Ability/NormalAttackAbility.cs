using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "NormalAttackAbility", menuName = "ScriptableObjects/NormalAttackAbility")]
    public class NormalAttackAbility : Ability
    {
        [field: SerializeField] public Projectile Projectile {get; set;}
        
        private void Reset()    
        {
            AbilityTrigger = Trigger.NormalAttack;
            AbilityTarget = Target.EnemyInRange;
        }

        public override bool CanUseAbility(Hero hero)
        {
            return true;
        }

        public override bool TryUseAbility(Hero hero)
        {
            List<Enemy> enemies = hero.GetAbilityTarget<Enemy>(AbilityTarget);
            if (enemies.Count == 0) return false;
            foreach (var enemy in enemies)
            {
                Projectile.Spawn(hero, enemy);
            }

            return true;
        }
    }
}