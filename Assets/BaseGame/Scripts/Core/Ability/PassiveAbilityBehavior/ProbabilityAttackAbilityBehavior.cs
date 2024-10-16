using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    [Serializable]
    public class ProbabilityAttackAbilityBehavior : PassiveAbilityBehavior
    {
        [field: SerializeField] private float Probability { get; set; }
        [field: SerializeField] public Projectile Projectile { get; set; }

        public override bool CanUseAbility(Hero hero, Ability.Trigger trigger, Ability.Target target)
        {
            return hero.CurrentAttackProbability <= Probability;
        }

        public override bool TryUseAbility(Hero hero, Ability.Trigger trigger, Ability.Target target)
        {
            List<Enemy> enemies = hero.GetAbilityTarget<Enemy>(target);
            if (enemies.Count == 0) return false;
            foreach (var enemy in enemies)
            {
                Projectile.Spawn(hero, enemy);
            }

            return true;
        }
    }
}