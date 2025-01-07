using System;
using System.Collections;
using System.Collections.Generic;
using MemoryPack;
using TW.Utility.DesignPattern;
using UnityEngine;

public class ShopManager : Singleton<ShopManager>
{
    public List<DailyDeal> dailyDeals = new();
    public DateTime DailyDealRefreshTime { get; set; } = new();
    private void Start()
    {
        LoadData();
    }

    void LoadData()
    {
        // Load data from save
        dailyDeals = InGameDataManager.Instance.InGameData.ShopIAPDataSave.dailyDeals;
        DailyDealRefreshTime = InGameDataManager.Instance.InGameData.ShopIAPDataSave.DailyDealRefreshTime.Value;
        
        if (DailyDealRefreshTime == default)
            DailyDealRefreshTime = DateTime.Now;
        
        TimeSpan timeSpan = DailyDealRefreshTime.Subtract(DateTime.Now);
        
        if (dailyDeals.Count == 0 || timeSpan.TotalHours <= 0)
        {
            dailyDeals = GetDailyDeals(6);
            InGameDataManager.Instance.SaveData();
        }
    }
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
[MemoryPackable]
public partial class DailyDeal {
    [field: SerializeField] public Resource PaymentAmount { get; set; }
    [field: SerializeField] public List<Resource> rewards { get; set; }
    [field: SerializeField] public List<HeroReward> heroRewards { get; set; }
}