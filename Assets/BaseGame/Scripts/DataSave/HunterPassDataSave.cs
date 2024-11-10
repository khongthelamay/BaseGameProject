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
    public ReactiveValue<bool> isPremium;
    public ReactiveList<HuntPassData> huntPassDatasSave = new();
    public ReactiveValue<string> timeOutHuntPass;
}
public partial class InGameData
{
    [field: SerializeField] public HuntPassDataSave huntPassDataSave { get; set; } = new();
}

[System.Serializable]
[MemoryPackable]
public partial class HuntPassData {
    public int level;
    public bool isClaimedCommond;
    public bool isClaimedPremium;
}
