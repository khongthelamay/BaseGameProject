using System;
using System.Collections.Generic;
using MemoryPack;
using TW.Reactive.CustomComponent;
using UnityEngine;

[System.Serializable]
[MemoryPackable]
public partial class ShopIAPDataSave
{
    public static ShopIAPDataSave Instance => InGameDataManager.Instance.InGameData.ShopIAPDataSave;
    
    public List<DailyDeal> dailyDeals = new();
    
    public ReactiveValue<DateTime> DailyDealRefreshTime { get; set; } = new();
}

public partial class InGameData
{
    [field: SerializeField] public ShopIAPDataSave ShopIAPDataSave { get; set; } = new();
}
