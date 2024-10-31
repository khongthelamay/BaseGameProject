using System;
using BaseGame.Scripts.Enum;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core
{
    [Serializable]
    public class DamageScaleBehavior : ProjectileBehavior
    {
        [field: Title(nameof(DamageScaleBehavior))]
        [field: SerializeField] public DamageType DamageType {get; private set;}

        [field: SerializeField] public float DamageScale {get; private set;}

        public override void StartBehavior(Hero hero, Enemy enemy)
        {
            if (enemy == null) return;
            int attackDamage = (int)(hero.HeroConfigData.BaseAttackDamage * DamageScale);
            enemy.MarkLoseHealth(attackDamage);
        }

        public override void EndBehavior(Hero hero, Enemy enemy)
        {
            if (enemy == null) return;
            int attackDamage = (int)(hero.HeroConfigData.BaseAttackDamage * DamageScale);
            enemy.TakeDamage(attackDamage, DamageType);
        }
        
        
    }
}