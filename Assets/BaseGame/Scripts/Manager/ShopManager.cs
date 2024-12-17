using System.Collections;
using System.Collections.Generic;
using TW.Utility.DesignPattern;
using UnityEngine;

public class ShopManager : Singleton<ArtifactManager>
{
    public List<DailyDeal> dailyDeals = new();

    public List<DailyDeal> GetDailyDeals(int countDailyDeal)
    {
        dailyDeals.Clear();
        for (int i = 0; i < countDailyDeal; i++)
        {
            DailyDeal dailyDeal = new();
            dailyDeals.Add(dailyDeal);
        }

        return dailyDeals;
    }

    public void BuyPack(PackID packID) {
        Debug.Log($"Buy Pack: {packID}");
    }
}

[System.Serializable]
public class DailyDeal {
    [field: SerializeField] public Resource PaymentAmount { get; set; }
    [field: SerializeField] public List<Resource> rewards { get; set; }
    [field: SerializeField] public List<HeroReward> heroRewards { get; set; }
}