using MemoryPack;
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
    public ReactiveValue<int> huntPoint = new(0);
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
    public bool isClaimedCommond;
    public bool isClaimedPremium;
}
