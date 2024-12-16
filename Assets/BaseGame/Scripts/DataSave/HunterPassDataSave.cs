using MemoryPack;
using System;
using System.Collections;
using System.Collections.Generic;
using TW.Reactive.CustomComponent;
using UnityEngine;

[System.Serializable]
[MemoryPackable]
public partial class HuntPassDataSave
{
    public static HuntPassDataSave Instance => InGameDataManager.Instance.InGameData.huntPassDataSave;
    public ReactiveValue<int> huntPassLevel = new(0);
    public ReactiveValue<float> huntPoint = new(0);
    public ReactiveList<HuntPassData> huntPassDatasSave = new();
    public ReactiveValue<string> timeOutHuntPass = new("");
}
public partial class InGameData
{
    [MemoryPackOrder(104)]
    [field: SerializeField] public HuntPassDataSave huntPassDataSave { get; set; } = new();
}

[System.Serializable]
[MemoryPackable]
public partial class HuntPassData {
    public int level;
    public ReactiveValue<bool> isClaimedCommond = new();
    public ReactiveValue<bool> isClaimedPremium = new();
}
