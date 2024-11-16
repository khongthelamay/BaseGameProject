using Core;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using TW.Reactive.CustomComponent;
using TW.Utility.DesignPattern;
using UnityEngine;

public class RecruitManager : Singleton<RecruitManager>
{
    public ReactiveList<RecruitReward> recruitRewards = new();
    public int currentTurn;
    public ReactiveValue<int> totalRewardCanGetThisTurn = new(0);
    RecruitRewardType recruitRewardTypeTemp = RecruitRewardType.Hero;
    List<HeroConfigData> currentHeroHave = new();
    RecruitTurn currentRecruitTurn;

    [Button]
    public void InitData(int recruitTotal) {

        if (currentTurn == 5) {
            currentTurn = 0;
            Debug.Log("Time out");
            return;
        }
        
        currentRecruitTurn = RecruitPersentGlobalConfig.Instance.GetRecruitTurn(currentTurn);

        recruitRewards.ObservableList.Clear();

        for (int i = 0; i < 10; i++)
        {
            //recruitRewardTypeTemp = Random.Range(0, 10) >= 5 ? RecruitRewardType.Item : RecruitRewardType.Hero;

            RecruitReward newRecruitReward = new();
            newRecruitReward.type = recruitRewardTypeTemp;

            switch (recruitRewardTypeTemp)
            {
                case RecruitRewardType.None:
                    break;
                case RecruitRewardType.Item:
                    newRecruitReward.resource = new();
                    break;
                case RecruitRewardType.Hero:
                    newRecruitReward.heroData = GetHeroRandom();
                    break;
                default:
                    break;
            }

            newRecruitReward.amount = recruitTotal;

            int randomRewardCanGet = Random.Range(0, 101);
            newRecruitReward.isMiss = randomRewardCanGet <= currentRecruitTurn.miss;

            recruitRewards.ObservableList.Add(newRecruitReward);
        }
        
        currentTurn++;
        totalRewardCanGetThisTurn.Value = 0;
    }

    HeroConfigData GetHeroRandom() {
        currentHeroHave = HeroPoolGlobalConfig.Instance.HeroConfigDataList;
        float randomHero = Random.Range(0, 101);

        Hero.Rarity rarity = Hero.Rarity.Common;

        for (int i = 0; i < currentRecruitTurn.rateHeroTiers.Count; i++)
        {
            if (randomHero < currentRecruitTurn.rateHeroTiers[i].persent)
            {
                rarity = currentRecruitTurn.rateHeroTiers[i].rarity;
                break;
            }
            else {
                randomHero -= currentRecruitTurn.rateHeroTiers[i].persent;
            }
        }

        currentHeroHave = currentHeroHave.FindAll(e=>e.HeroRarity == rarity);
        if (currentHeroHave.Count == 0)
            return null;

        return currentHeroHave[Random.Range(0, currentHeroHave.Count)];
    }
}
