using Core;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TW.Reactive.CustomComponent;
using TW.Utility.DesignPattern;
using UnityEngine;

public class RecruitManager : Singleton<RecruitManager>
{
    public ReactiveList<RecruitReward> recruitRewards = new();
    public ReactiveList<RecruitReward> recruitRewardEarned = new();
    public int currentTurn;
    public ReactiveValue<int> totalRewardNeedGetThisTurn = new(0);
    RecruitRewardType recruitRewardTypeTemp = RecruitRewardType.Hero;
    List<HeroConfigData> currentHeroHave = new();
    RecruitTurn currentRecruitTurn;

    [Button]
    public void InitData(int recruitTotal) {

        if (currentTurn == 5) {
            currentTurn = 0;
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
            if (!newRecruitReward.isMiss)
                AddRewardEarned(newRecruitReward);
            recruitRewards.ObservableList.Add(newRecruitReward);
        }
        totalRewardNeedGetThisTurn.Value = 5 + currentTurn;
        currentTurn++;
    }

    void AddRewardEarned(RecruitReward recruitReward) {
        if (recruitReward.type == RecruitRewardType.Item)
            return;

        for (int i = 0; i < recruitRewardEarned.ObservableList.Count; i++)
        {
            if (recruitRewardEarned.ObservableList[i].type == recruitReward.type)
            {
                if (recruitRewardEarned.ObservableList[i].heroData.Name == recruitReward.heroData.Name)
                {
                    recruitRewardEarned.ObservableList[i].amount += recruitReward.amount;
                    return;
                }
            }
        }
        RecruitReward newRecruitReward = new();
        newRecruitReward.type = recruitReward.type;
        newRecruitReward.heroData = recruitReward.heroData;
        newRecruitReward.resource = recruitReward.resource;
        newRecruitReward.amount = recruitReward.amount;
        recruitRewardEarned.ObservableList.Add(newRecruitReward);

    }

    public void ResetTurn() { currentTurn = 0; }

    public bool IsCanContinue()
    {
        int totalRewardEarn = 0;
        for (int i = 0; i < recruitRewards.ObservableList.Count; i++)
        {
            if (!recruitRewards.ObservableList[i].isMiss)
                totalRewardEarn++;
        }

        return totalRewardEarn >= totalRewardNeedGetThisTurn;
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

    public void ClearRewardEarned()
    {
        recruitRewardEarned.ObservableList.Clear();
    }
}
