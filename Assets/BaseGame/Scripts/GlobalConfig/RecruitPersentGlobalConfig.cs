using UnityEngine;
using Sirenix.Utilities;
using System.Collections.Generic;
using Core;
using Sirenix.OdinInspector;
using TW.Utility.Extension;

[CreateAssetMenu(fileName = "RecruitPersentGlobalConfig", menuName = "GlobalConfigs/RecruitPersentGlobalConfig")]
[GlobalConfig("Assets/Resources/GlobalConfig/")]
public class RecruitPersentGlobalConfig : GlobalConfig<RecruitPersentGlobalConfig>
{
    public List<RecruitTurn> recruitTurns = new();

    public RecruitTurn GetRecruitTurn(int turn) { return recruitTurns[turn]; }

#if UNITY_EDITOR

    string linkSheetId = "1-HkinUwSW4A4SkuiLGtl0Tm8771jFPVZB5ZpLs5pxz4";

    string requestedData;

    [Button]
    public void FetchQuestData()
    {
        if (string.IsNullOrEmpty(linkSheetId)) return;
        FetchAchievement();
    }

    async void FetchAchievement()
    {
        requestedData = await ABakingSheet.GetCsv(linkSheetId, "Gacha");

        List<Dictionary<string, string>> data = ACsvReader.ReadDataFromString(requestedData);

        recruitTurns.Clear();

        for (int i = 0; i < data.Count; i++)
        {
            RecruitTurn newRecreuitTurn = new();
            newRecreuitTurn.turn = i;
            newRecreuitTurn.miss = float.Parse(data[i]["Miss"]);
            newRecreuitTurn.rateHeroTiers = new();

            RateHeroTier rateHeroTier1 = new();
            rateHeroTier1.persent = float.Parse(data[i]["Tier1"]);
            rateHeroTier1.rarity = Hero.Rarity.Common;

            RateHeroTier rateHeroTier2 = new();
            rateHeroTier2.persent = float.Parse(data[i]["Tier2"]);
            rateHeroTier2.rarity = Hero.Rarity.Rare;

            RateHeroTier rateHeroTier3 = new();
            rateHeroTier3.persent = float.Parse(data[i]["Tier3"]);
            rateHeroTier3.rarity = Hero.Rarity.Epic;

            RateHeroTier rateHeroTier4 = new();
            rateHeroTier4.persent = float.Parse(data[i]["Tier4"]);
            rateHeroTier4.rarity = Hero.Rarity.Legendary;

            RateHeroTier rateHeroTier5 = new();
            rateHeroTier5.persent = float.Parse(data[i]["Tier5"]);
            rateHeroTier5.rarity = Hero.Rarity.Mythic;

            newRecreuitTurn.rateHeroTiers.Add(rateHeroTier1);
            newRecreuitTurn.rateHeroTiers.Add(rateHeroTier2);
            newRecreuitTurn.rateHeroTiers.Add(rateHeroTier3);
            newRecreuitTurn.rateHeroTiers.Add(rateHeroTier4);
            newRecreuitTurn.rateHeroTiers.Add(rateHeroTier5);

            recruitTurns.Add(newRecreuitTurn);
        }
    }
#endif
}

[System.Serializable]
public class RecruitTurn {
    public int turn;
    public float miss;
    public List<RateHeroTier> rateHeroTiers = new();
}

[System.Serializable]
public class RateHeroTier {
    public Hero.Rarity rarity;
    public float persent;
}

public enum RecruitRewardType { 
    None,
    Item, 
    Hero
}

[System.Serializable]
public class RecruitReward {
    public RecruitRewardType type;
    [ShowIf("type", RecruitRewardType.Item)] public Resource resource;
    [ShowIf("type", RecruitRewardType.Hero)] public Hero heroData;
    public int amount;
}