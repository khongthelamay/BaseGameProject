using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using TW.Utility.CustomComponent;
using TW.Utility.DesignPattern;
using UnityEngine;
using Zenject;

namespace Manager
{
    public delegate List<IAbilityTargetAble> GetAbilityTarget(Hero hero);
    public class TargetingManager : Singleton<TargetingManager>
    {
        [Inject] private BattleManager BattleManager { get; set; }
        private Dictionary<Ability.Target, GetAbilityTarget> GetAbilityTargetDictionary { get; set; } = new();
        protected override void Awake()
        {
            base.Awake();
            GetAbilityTargetDictionary.Add(Ability.Target.Self, GetSelf);
            GetAbilityTargetDictionary.Add(Ability.Target.EnemyInRange, GetEnemyInRange);
            GetAbilityTargetDictionary.Add(Ability.Target.AllEnemiesInRange, GetAllEnemiesInRange);
            GetAbilityTargetDictionary.Add(Ability.Target.AllyInRange, GetAllyInRange);
            GetAbilityTargetDictionary.Add(Ability.Target.AllAlliesInRange, GetAllAlliesInRange);
        }
        public List<IAbilityTargetAble> GetAbilityTarget(Hero hero, Ability.Target target)
        {
            return GetAbilityTargetDictionary[target](hero);
        }
        public List<T> GetAbilityTarget<T>(Hero hero, Ability.Target target) where T : IAbilityTargetAble
        {
            return GetAbilityTargetDictionary[target](hero).OfType<T>().ToList();
        }
        
        private List<IAbilityTargetAble> GetSelf(Hero hero)
        {
            return new List<IAbilityTargetAble>() {hero};
        }

        private List<IAbilityTargetAble> GetEnemyInRange(Hero hero)
        {
            foreach (Enemy enemy in BattleManager.EnemyList)
            {
                if (enemy.WillBeDead)continue;
                if ((hero.Transform.position - enemy.Transform.position).sqrMagnitude > hero.AttackRange * hero.AttackRange) continue;
                return new List<IAbilityTargetAble>() {enemy};
            }
            return new List<IAbilityTargetAble>();
        }
        private List<IAbilityTargetAble> GetAllEnemiesInRange(Hero hero)
        {
            List<IAbilityTargetAble> enemyList = new();
            foreach (Enemy enemy in BattleManager.EnemyList)
            {
                if (enemy.WillBeDead)continue;
                if ((hero.Transform.position - enemy.transform.position).sqrMagnitude > hero.AttackRange * hero.AttackRange) continue;
                enemyList.Add(enemy);
            }
            return enemyList;
        }
        private List<IAbilityTargetAble> GetAllyInRange(Hero hero)
        {
            List<IAbilityTargetAble> heroList = new();
            foreach (FieldSlot fieldSlot in BattleManager.CurrentMap.FieldSlotArray)
            {
                if (fieldSlot.TryGetHero(out Hero fieldHero)) continue;
                if (fieldHero == hero) continue;
                if ((hero.Transform.position - fieldHero.Transform.position).sqrMagnitude > hero.AttackRange * hero.AttackRange) continue;
                return new List<IAbilityTargetAble>() {fieldHero};
            }
            return heroList;
        }
        private List<IAbilityTargetAble> GetAllAlliesInRange(Hero hero)
        {
            List<IAbilityTargetAble> heroList = new();
            foreach (FieldSlot fieldSlot in BattleManager.CurrentMap.FieldSlotArray)
            {
                if (fieldSlot.TryGetHero(out Hero fieldHero)) continue;
                if ((hero.Transform.position - fieldHero.Transform.position).sqrMagnitude > hero.AttackRange * hero.AttackRange) continue;
                heroList.Add(fieldSlot.Hero);
            }
            return heroList;
        }
    }
}