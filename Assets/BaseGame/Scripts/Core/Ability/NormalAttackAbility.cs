using System;
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
                if (Projectile)
                {
                    RangeAttack(hero, enemy);
                }
                else
                {
                    MeleeAttack(hero, enemy);
                }
            }

            return true;
        }
        
        private void RangeAttack(Hero hero, Enemy enemy)
        {
            if (!Projectile) return;
            // TODO: Attack animation
            Instantiate(Projectile, hero.Transform.position, Quaternion.identity)
                .Init(hero, enemy, hero.HeroStatData.BaseAttackDamage);
        }
        
        private void MeleeAttack(Hero hero, Enemy enemy)
        {
            if (Projectile) return;
            // TODO: Attack animation
            enemy.TakeDamage(hero.HeroStatData.BaseAttackDamage);
        }
    }
}