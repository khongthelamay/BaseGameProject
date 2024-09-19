using System;
using System.Collections.Generic;
using Core;
using Cysharp.Threading.Tasks;
using TW.Utility.DesignPattern;
using UnityEngine;

namespace Manager
{
    public class BattleManager : Singleton<BattleManager>
    {
        [field: SerializeField] public Map CurrentMap {get; private set;}
        public List<Enemy> EnemyList { get; private set; } = new();

        private void Start()
        {
            StartBattle();
        }
        public void StartBattle()
        {
            CurrentMap.StartMap().Forget();
        }
        public void RemoveEnemy(Enemy enemy)
        {
            if (!EnemyList.Contains(enemy)) return;
            EnemyList.Remove(enemy);
        }
        public void AddEnemy(Enemy enemy)
        {
            if (EnemyList.Contains(enemy)) return;
            EnemyList.Add(enemy);
        }
        public bool TryAddNewHero(Hero hero)
        {
            foreach (FieldSlot fieldSlot in CurrentMap.FieldSlotArray)
            {
                if (fieldSlot.TryAddHero(hero))
                {
                    return true;
                }
            }
            return false;
        }
        public bool TrySellHero(FieldSlot fieldSlot)
        {
            Debug.Log("Sell Hero");
            if (fieldSlot.TryRemoveHero())
            {
                OnFieldSlotChange();
                return true;
            }
            return false;
        }
        public bool TryFusionHeroInFieldSlot(FieldSlot fieldSlot)
        {
            if (!TryGetSameHeroInFieldSlot(fieldSlot, out List<FieldSlot> sameHeroFieldSlotList)) return false;
            if (sameHeroFieldSlotList.Count < 2) return false;
            foreach (var slot in sameHeroFieldSlotList)
            {
                slot.RemoveHero();
            }

            if (fieldSlot.TryUpgradeHero())
            {
                OnFieldSlotChange();
            }
            return false;
        }
        private bool TryGetSameHeroInFieldSlot(FieldSlot fieldSlot, out List<FieldSlot> sameHeroFieldSlotList)
        {
            sameHeroFieldSlotList = new List<FieldSlot>();
            if (!fieldSlot.TryGetHero(out Hero hero)) return false;
            foreach (FieldSlot slot in CurrentMap.FieldSlotArray)
            {
                if (slot == fieldSlot) continue;
                if (!IsFieldSlotHasSameHeroData(slot, hero))
                {
                    continue;
                }
                sameHeroFieldSlotList.Add(slot);
                if (sameHeroFieldSlotList.Count == 2) break;
            }
            return sameHeroFieldSlotList.Count == 2;
        }
        private bool IsFieldSlotHasSameHeroData(FieldSlot fieldSlot, Hero hero)
        {
            return fieldSlot.TryGetHero(out Hero otherHero) && otherHero.HeroStatData == hero.HeroStatData;
        }
        private void OnFieldSlotChange()
        {
            foreach (FieldSlot slot in CurrentMap.FieldSlotArray)
            {
                slot.SetUpgradeMark(TryGetSameHeroInFieldSlot(slot, out List<FieldSlot> _));
            }
        }
        
    }
}