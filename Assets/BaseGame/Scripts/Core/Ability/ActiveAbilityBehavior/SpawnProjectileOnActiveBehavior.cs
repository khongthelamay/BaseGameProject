using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    [Serializable]
    public class SpawnProjectileOnActiveBehavior : ActiveAbilityBehavior
    {
        // [field: SerializeField] public int ArrowCount {get; private set;}
        [field: SerializeField] public Projectile Projectile {get; private set;}

        public override bool CanUseAbility(Hero hero, float cooldown, Ability.Trigger trigger, Ability.Target target)
        {
            return true;
        }

        public override bool TryUseAbility(Hero hero, float cooldown, Ability.Trigger trigger, Ability.Target target)
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