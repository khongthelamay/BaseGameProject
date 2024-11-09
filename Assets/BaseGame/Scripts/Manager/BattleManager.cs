using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.SimplePool;
using Cysharp.Threading.Tasks;
using R3;
using R3.Triggers;
using Sirenix.OdinInspector;
using TW.Utility.DesignPattern;
using UnityEngine;

namespace Manager
{
    public partial class BattleManager : Singleton<BattleManager>
    {
        [field: SerializeField] public Map CurrentMap { get; private set; }
        [field: SerializeField] public TickRate TickRate {get; private set;}
        [field: SerializeField] public WaitSlot[] WaitSlotArray {get; private set;}
        private Dictionary<GlobalBuff.Type, GlobalBuff> GlobalBuffDictionary { get; set; } = new();
        private List<Enemy> EnemyList { get; set; } = new();
        private List<Hero> HeroList { get; set; } = new();
        [ShowInInspector]
        private List<GlobalBuff> GlobalBuffList => GlobalBuffDictionary.Values.ToList();
    
        private void Start()
        {
#if !UNITY_EDITOR
            Application.targetFrameRate = 60;
#endif
            InitGlobalBuff();
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
        private void ClearAllGlobalBuff()
        {
            foreach (var globalBuff in GlobalBuffDictionary.Values)
            {
                globalBuff.ChangeValue(0);
            }
        }
        
        
        public void StartNewMatch()
        {
            ClearAllGlobalBuff();
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
            selectHero.SelfDespawn();
            
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
            return fieldSlot.TryGetHero(out Hero otherHero) && otherHero.HeroConfigData == hero.HeroConfigData;
        }
        public bool TryGetEnemyInAttackRange(Hero hero, out Enemy enemy)
        {
            enemy = null;
            foreach (var e in EnemyList)
            {
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
                if ((position - e.Transform.position).sqrMagnitude <= radius * radius)
                {
                    enemies[count] = e;
                    count++;
                }
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
    }
}