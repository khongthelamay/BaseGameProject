using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.SimplePool;
using Cysharp.Threading.Tasks;
using R3;
using R3.Triggers;
using Sirenix.OdinInspector;
using TW.Reactive.CustomComponent;
using TW.Utility.DesignPattern;
using UnityEngine;

namespace Manager
{
    public partial class BattleManager : Singleton<BattleManager>
    {
        private FactoryManager FactoryManagerCache { get; set; }
        private FactoryManager FactoryManager => FactoryManagerCache ??= FactoryManager.Instance;
        [field: SerializeField] public Map CurrentMap { get; private set; }
        [field: SerializeField] public TickRate TickRate {get; private set;}
        [field: SerializeField] public int MaxHeroInField {get; private set;}
        [field: SerializeField] public GameResource CoinResource {get; private set;}
        [field: SerializeField] public GameResource StoneResource {get; private set;}

        [field: SerializeField] public ReactiveValue<int> CommonRareLevel {get; private set;}
        [field: SerializeField] public ReactiveValue<int> EpicLevel {get; private set;}
        [field: SerializeField] public ReactiveValue<int> LegendaryMythicLevel {get; private set;}
        [field: SerializeField] public ReactiveValue<int> ShopLevel {get; private set;}

        [field: SerializeField] public WaitSlot[] WaitSlotArray {get; private set;}
        
        private Dictionary<GlobalBuff.Type, GlobalBuff> GlobalBuffDictionary { get; set; } = new();
        
        private List<Enemy> EnemyList { get; set; } = new();
        private List<Hero> HeroList { get; set; } = new();
        [ShowInInspector] private List<GlobalBuff> GlobalBuffList => GlobalBuffDictionary.Values.ToList();
        
        private void Start()
        {
#if !UNITY_EDITOR
            Application.targetFrameRate = 60;
#endif
            InitGlobalBuff();
            InitResource();
            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(KeyCode.C))
                .Subscribe(_ => ReRollWaitSlot());
        }
        private void InitGlobalBuff()
        {
            IEnumerable<GlobalBuff.Type> allGlobalBuffType = Enum.GetValues(typeof(GlobalBuff.Type)).Cast<GlobalBuff.Type>();
            foreach (GlobalBuff.Type type in allGlobalBuffType)
            {
                GlobalBuffDictionary.Add(type, new GlobalBuff(type, 0));
            }
        }
        private void InitResource()
        {
            CoinResource = new GameResource(GameResource.Type.CoinInMatch, 0);
            StoneResource = new GameResource(GameResource.Type.StoneInMatch, 0);
        }
        private void ClearAllGlobalBuff()
        {
            foreach (var globalBuff in GlobalBuffDictionary.Values)
            {
                globalBuff.ChangeValue(0);
            }
        }
        private void ClearUpgradeBuff()
        {
            CommonRareLevel.Value = 1;
            EpicLevel.Value = 1;
            LegendaryMythicLevel.Value = 1;
            ShopLevel.Value = 1;
            
            CoinResource.Amount = 100000;
            StoneResource.Amount = 100000;
        }
        
        public void StartNewMatch()
        {
            ClearAllGlobalBuff();
            ClearUpgradeBuff();
            CurrentMap.StartMap().Forget();
            ReRollWaitSlot();
        }
        public void ReRollWaitSlot()
        {
            foreach (var waitSlot in WaitSlotArray)
            {
                waitSlot.RandomOwnerHero();
            }
        }

        public Hero RegisterHero(Hero hero)
        {
            if (HeroList.Contains(hero)) return hero;
            HeroList.Add(hero);
            return hero;
        }
        public Hero UnregisterHero(Hero hero)
        {
            if (!HeroList.Contains(hero)) return hero;
            HeroList.Remove(hero);
            return hero;
        }
        public Enemy RegisterEnemy(Enemy enemy)
        {
            if (EnemyList.Contains(enemy)) return enemy;
            EnemyList.Add(enemy);
            return enemy;
        }
        public Enemy UnregisterEnemy(Enemy enemy)
        {
            if (!EnemyList.Contains(enemy)) return enemy;
            EnemyList.Remove(enemy);
            return enemy;
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
                hero.Despawn();
                return true;
            }

            return false;
        }

        public bool CanFusionHeroInFieldSlot(FieldSlot fieldSlot)
        {
            if (fieldSlot.TryGetHero(out Hero selectHero) && selectHero.HeroConfigData.HeroRarity.IsMaxRarity()) return false;
            return HasSame2HeroInOtherFieldSlot(fieldSlot);
        }
        public async UniTask FusionHeroInFieldSlot(FieldSlot fieldSlot)
        {
            if (!fieldSlot.TryGetHero(out Hero selectHero)) return;
            HeroConfigData heroConfigData = selectHero.HeroConfigData;
            if (heroConfigData.HeroRarity == Hero.Rarity.Mythic) return;
            
            
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
            selectHero.Despawn();
            
            HeroConfigData newHeroConfigData = HeroPoolGlobalConfig.Instance.GetRandomHeroConfigDataUpgrade(heroConfigData.HeroRarity + 1);
            Hero newHero = newHeroConfigData.HeroPrefab.Spawn();
            newHero.Transform.position = fieldSlot.Transform.position;
            fieldSlot.TryAddHero(newHero);
            FieldSlot.FieldSlotChangedCallback?.Invoke();
            
            return;
            bool AllHeroDespawn()
            {
                return sameHeroList.All(hero => hero == null);
            }
        }
        private bool HasSame2HeroInOtherFieldSlot(FieldSlot fieldSlot)
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
            return fieldSlot.TryGetHero(out Hero otherHero) && otherHero.HeroConfigData == hero.HeroConfigData;
        }
        public bool TryGetEnemyInAttackRange(Hero hero, out Enemy enemy)
        {
            enemy = null;
            foreach (var e in EnemyList)
            {
                if (e.WillBeDead) continue;
                if (e.IsDead) continue;
                if ((hero.Transform.position - e.Transform.position).sqrMagnitude <= hero.AttackRange * hero.AttackRange)
                {
                    enemy = e;
                    return true;
                }
            }
            return false;
        }
        public int GetEnemyAroundNonAlloc(Vector3 position, float radius, Enemy[] enemies)
        {
            int count = 0;
            foreach (var e in EnemyList)
            {
                if (count >= enemies.Length) break;
                if (e.WillBeDead) continue;
                if (e.IsDead) continue;
                if ((position - e.Transform.position).sqrMagnitude <= radius * radius)
                {
                    enemies[count] = e;
                    count++;
                }
            }
            return count;
        }
        public int GetAlliesAroundNonAlloc(Hero hero, Hero[]  heroes)
        {
            int count = 0;
            foreach (var h in HeroList)
            {
                if (count >= heroes.Length) break;
                if (h == hero) continue;
                if (!h.IsInBattleState()) continue; 
                heroes[count] = h;
                count++;
            }

            return count;
        }
        public GlobalBuff GetGlobalBuff(GlobalBuff.Type type)
        {
            return GlobalBuffDictionary[type];
        }
        public void ChangeGlobalBuff(GlobalBuff.Type type, float value)
        {
            GlobalBuffDictionary[type].ChangeValue(value);
        }
        public int GetFieldSlotHasSameHeroData(FieldSlot fieldSlot, FieldSlot[] fieldSlots, Hero[] heroes)
        {
            int count = 0;
            foreach (FieldSlot slot in CurrentMap.FieldSlotArray)
            {
                if (count == fieldSlots.Length) break;
                if (slot == fieldSlot) continue;
                if (slot.TryGetHero(out Hero hero) && hero.HeroConfigData == fieldSlot.Hero.HeroConfigData)
                {
                    fieldSlots[count] = slot;
                    heroes[count] = hero;
                    count++;
                }
            }
            return count;
        }
        public void UpgradeCommonRareLevel()
        {
            CommonRareLevel.Value++;
            foreach (Hero hero in HeroList)
            {
                if(!hero.IsInBattleState()) continue;
                if (hero.HeroRarity != Hero.Rarity.Common && hero.HeroRarity != Hero.Rarity.Rare) continue;
                FactoryManager.CreateUpgradeEffect(hero.Transform.position, hero.HeroRarity);
            }
        }
        public void UpgradeEpicLevel()
        {
            EpicLevel.Value++;
            foreach (Hero hero in HeroList)
            {
                if(!hero.IsInBattleState()) continue;
                if (hero.HeroRarity != Hero.Rarity.Epic) continue;
                FactoryManager.CreateUpgradeEffect(hero.Transform.position, hero.HeroRarity);
            }
        }
        public void UpgradeLegendaryMythicLevel()
        {
            LegendaryMythicLevel.Value++;
            foreach (Hero hero in HeroList)
            {
                if(!hero.IsInBattleState()) continue;
                if (hero.HeroRarity != Hero.Rarity.Legendary && hero.HeroRarity != Hero.Rarity.Mythic) continue;
                FactoryManager.CreateUpgradeEffect(hero.Transform.position, hero.HeroRarity);
            }
        }
        public void UpgradeShopLevel()
        {
            ShopLevel.Value++;
        }
        public int GetHeroRarityUpgrade(Hero.Rarity rarity)
        {
            return rarity switch
            {
                Hero.Rarity.Common => CommonRareLevel.Value,
                Hero.Rarity.Rare => CommonRareLevel.Value,
                Hero.Rarity.Epic => EpicLevel.Value,
                Hero.Rarity.Legendary => LegendaryMythicLevel.Value,
                Hero.Rarity.Mythic => LegendaryMythicLevel.Value,
                _ => 0
            };
        }
    }
}