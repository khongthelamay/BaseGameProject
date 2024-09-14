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