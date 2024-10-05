using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Cysharp.Threading.Tasks;
using TW.Utility.DesignPattern;
using UnityEngine;
using Zenject;

namespace Manager
{
    public class BattleManager : Singleton<BattleManager>
    {
        [Inject] Hero.Factory HeroFactory { get; }
        [field: SerializeField] public Map CurrentMap { get; private set; }
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
                    FieldSlot.FieldSlotChangedCallback?.Invoke();
                    return true;
                }
            }

            return false;
        }

        public bool TrySellHero(FieldSlot fieldSlot)
        {
            Debug.Log("Sell Hero");
            if (fieldSlot.TryRemoveHero(out Hero hero))
            {
                hero.SelfDespawn();
                return true;
            }

            return false;
        }

        // public bool TryFusionHeroInFieldSlot(FieldSlot fieldSlot)
        // {
        //     if (fieldSlot.TryGetHero(out Hero selectHero) && selectHero.HeroStatData.HeroRarity.IsMaxRarity())
        //         return false;
        //     if (!TryGetSameHeroInFieldSlot(fieldSlot, out List<FieldSlot> sameHeroFieldSlotList)) return false;
        //     if (sameHeroFieldSlotList.Count < 2) return false;
        //
        //
        //     if (fieldSlot.TryUpgradeHero())
        //     {
        //         FieldSlot.FieldSlotChangedCallback?.Invoke();
        //     }
        //
        //     return true;
        // }

        public bool CanFusionHeroInFieldSlot(FieldSlot fieldSlot)
        {
            if (fieldSlot.TryGetHero(out Hero selectHero) && selectHero.HeroStatData.HeroRarity.IsMaxRarity()) return false;
            return HasSame2HeroInOtherFieldSlot(fieldSlot);
        }

        public async UniTask FusionHeroInFieldSlot(FieldSlot fieldSlot)
        {
            if (!fieldSlot.TryGetHero(out Hero selectHero)) return;
            HeroStatData heroStatData = selectHero.HeroStatData;
            if (heroStatData.HeroRarity == Hero.Rarity.Mythic) return;
            
            
            if (!TryGetSameHeroInFieldSlot(fieldSlot, out List<FieldSlot> sameHeroFieldSlotList)) return;
            List<Hero> sameHeroList = new List<Hero>();
            foreach (FieldSlot slot in sameHeroFieldSlotList)
            {
                if (slot.TryRemoveHero(out Hero hero))
                {
                    slot.SetUpgradeMark(false);
                    hero.MoveToPositionAndSelfDespawn(fieldSlot.Transform.position);
                    sameHeroList.Add(hero);
                }
            }
            selectHero.ChangeToSleepState();
            await UniTask.WaitUntil(AllHeroDespawn, cancellationToken: this.GetCancellationTokenOnDestroy());
            
            fieldSlot.TryRemoveHero(out _);
            selectHero.SelfDespawn();
            
            Hero heroPrefab = HeroPoolGlobalConfig.Instance.GetRandomHeroUpgradePrefab(heroStatData.HeroRarity + 1);
            Hero newHero = HeroFactory.Create(heroPrefab);
            newHero.Transform.position = fieldSlot.Transform.position;
            fieldSlot.TryAddHero(newHero);
            FieldSlot.FieldSlotChangedCallback?.Invoke();
            
            return;
            bool AllHeroDespawn()
            {
                return sameHeroList.All(hero => hero == null);
            }
        }


        public bool HasSame2HeroInOtherFieldSlot(FieldSlot fieldSlot)
        {
            if (!fieldSlot.TryGetHero(out Hero hero)) return false;
            int count = 0;
            foreach (FieldSlot slot in CurrentMap.FieldSlotArray)
            {
                if (slot == fieldSlot) continue;
                if (!IsFieldSlotHasSameHeroData(slot, hero)) continue;
                count++;
                if (count == 2) break;
            }

            return count == 2;
        }

        public bool TryGetSameHeroInFieldSlot(FieldSlot fieldSlot, out List<FieldSlot> sameHeroFieldSlotList)
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
    }
}