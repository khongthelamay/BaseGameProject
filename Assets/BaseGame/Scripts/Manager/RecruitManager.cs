using Core;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TW.Reactive.CustomComponent;
using TW.Utility.CustomType;
using TW.Utility.DesignPattern;
using UnityEngine;

public class RecruitManager : Singleton<RecruitManager>
{
    public ReactiveList<RecruitReward> recruitRewards = new();
    public ReactiveList<RecruitReward> recruitHeroRewardEarned = new();
    public ReactiveList<RecruitReward> recruitItemRewardEarned = new();
    public int currentTurn;
    public ReactiveValue<int> totalRewardNeedGetThisTurn = new(0);
    RecruitRewardType recruitRewardTypeTemp = RecruitRewardType.Hero;
    List<HeroConfigData> currentHeroHave = new();
    RecruitTurn currentRecruitTurn;

    public ReactiveValue<BigNumber> recruitRecipe;

    private void Start()
    {
        LoadData();
    }

    void LoadData() {
        recruitRecipe = InGameDataManager.Instance.InGameData.playerResourceDataSave.GetResourceValue(ResourceType.RecruitRecipe);
    }

    [Button]
    public void InitData(int recruitTotal) {

        if (currentTurn == 5) {
            currentTurn = 0;
            return;
        }

        InGameDataManager.Instance.InGameData.playerResourceDataSave.ConsumeResourceValue(ResourceType.RecruitRecipe, (recruitTotal * 30));
        
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
        {
            AddItemReward(recruitReward);
        }
        else
            AddHeroReward(recruitReward);
    }

    void AddItemReward(RecruitReward recruitReward)
    {
        for (int i = 0; i < recruitItemRewardEarned.ObservableList.Count; i++)
        {
            if (recruitItemRewardEarned.ObservableList[i].type == recruitReward.type)
            {
                if (recruitItemRewardEarned.ObservableList[i].heroData.Name == recruitReward.heroData.Name)
                {
                    recruitItemRewardEarned.ObservableList[i].amount += recruitReward.amount;
                    return;
                }
            }
        }
        RecruitReward newRecruitReward = new();
        newRecruitReward.type = recruitReward.type;
        newRecruitReward.heroData = recruitReward.heroData;
        newRecruitReward.resource = recruitReward.resource;
        newRecruitReward.amount = recruitReward.amount;
        recruitItemRewardEarned.ObservableList.Add(newRecruitReward);
    }

    void AddHeroReward(RecruitReward recruitReward) {
        for (int i = 0; i < recruitHeroRewardEarned.ObservableList.Count; i++)
        {
            if (recruitHeroRewardEarned.ObservableList[i].type == recruitReward.type)
            {
                if (recruitHeroRewardEarned.ObservableList[i].heroData.Name == recruitReward.heroData.Name)
                {
                    recruitHeroRewardEarned.ObservableList[i].amount += recruitReward.amount;
                    return;
                }
            }
        }
        RecruitReward newRecruitReward = new();
        newRecruitReward.type = recruitReward.type;
        newRecruitReward.heroData = recruitReward.heroData;
        newRecruitReward.resource = recruitReward.resource;
        newRecruitReward.amount = recruitReward.amount;
        recruitHeroRewardEarned.ObservableList.Add(newRecruitReward);
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

    public void ClearHeroRewardEarned()
    {
        recruitHeroRewardEarned.ObservableList.Clear();
    }

    public void ClearItemRewardEarned() {
        recruitItemRewardEarned.ObservableList.Clear();
    }

    public bool IsHaveHeroReward()
    {
        return recruitHeroRewardEarned.ObservableList.Count > 0;
    }

    public bool IsHaveItemReward()
    {
        return recruitItemRewardEarned.ObservableList.Count > 0;
    }
}
