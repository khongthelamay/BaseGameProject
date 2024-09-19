using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "ProbabilityAttack", menuName = "ScriptableObjects/ProbabilityAttack")]
    public class ProbabilityAttack : Ability
    {
        [field: SerializeField] private float Probability { get; set; }
        [field: SerializeField] public Projectile Projectile { get; private set; }

        private void Reset()
        {
            AbilityTrigger = Trigger.ProbabilityAttack;
        }

        public override bool CanUseAbility(Hero hero)
        {
            return hero.CurrentAttackProbability <= Probability;
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