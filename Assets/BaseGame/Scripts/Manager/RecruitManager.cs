using Core;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TW.Utility.DesignPattern;
using UnityEngine;

public class RecruitManager : Singleton<RecruitManager>
{
    public List<RecruitReward> recruitRewards = new();
    public int currentTurn;
    RecruitRewardType recruitRewardTypeTemp = RecruitRewardType.Hero;
    List<HeroConfigData> currentHeroHave = new();
    RecruitTurn currentRecruitTurn;

    [Button]
    public void InitData(int recruitTotal) {

        
        currentRecruitTurn = RecruitPersentGlobalConfig.Instance.GetRecruitTurn(currentTurn);

        recruitRewards.Clear();

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

            recruitRewards.Add(newRecruitReward);
        }
        
        currentTurn++;
    }

    HeroConfigData GetHeroRandom() {
        currentHeroHave = HeroPoolGlobalConfig.Instance.HeroConfigDataList;
        float randomHero = Random.Range(0, 101);
        Debug.Log("=============================");
        Debug.Log("Hero random is: "+ randomHero);

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
        Debug.Log("Hero random last is: " + randomHero);

        currentHeroHave = currentHeroHave.FindAll(e=>e.HeroRarity == rarity);
        if (currentHeroHave.Count == 0)
            return null;

        return currentHeroHave[Random.Range(0, currentHeroHave.Count)];
    }
}
