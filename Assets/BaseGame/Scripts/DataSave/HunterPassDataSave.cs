using MemoryPack;
using System.Collections;
using System.Collections.Generic;
using TW.Reactive.CustomComponent;
using UnityEngine;

[System.Serializable]
[MemoryPackable]
public partial class HunterPassDataSave
{
    public static HunterPassDataSave Instance => InGameDataManager.Instance.InGameData.hunterPassDataSave;
    public ReactiveList<HunterPassData> hunterPassDataSave = new();
}
public partial class InGameData
{
    [field: SerializeField] public HunterPassDataSave hunterPassDataSave { get; set; } = new();
}

[System.Serializable]
[MemoryPackable]
public partial class HunterPassData {
    public int level;
    public bool isClaimedCommond;
    public bool isClaimedPremium;
}
