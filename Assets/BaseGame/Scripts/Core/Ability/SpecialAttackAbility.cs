using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "SpecialAttackAbility", menuName = "ScriptableObjects/SpecialAttackAbility")]
    public class SpecialAttackAbility : Ability
    {
        [field: SerializeField] private int AfterNormalAttack {get; set;}
        [field: SerializeField] public Projectile Projectile {get; private set;}
        
        private void Reset()    
        {
            AbilityTrigger = Trigger.SpecialAttack;
        }
        
        public override bool CanUseAbility(Hero hero)
        {
            return hero.CurrentAttackCount % AfterNormalAttack == 0;
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