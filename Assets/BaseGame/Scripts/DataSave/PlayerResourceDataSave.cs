using MemoryPack;
using System;
using System.Collections.Generic;
using TW.Reactive.CustomComponent;
using TW.Utility.CustomType;
using UnityEngine;

[System.Serializable]
[MemoryPackable]
public partial class PlayerResourceDataSave
{
    public static PlayerResourceDataSave Instance => InGameDataManager.Instance.InGameData.playerResourceDataSave;
    [field: SerializeField] public ReactiveValue<int> level = new(0);
    [field: SerializeField] public ReactiveValue<float> exp = new(0);
    [field: SerializeField] public ReactiveValue<bool> premium = new();
    [field: SerializeField] public List<ResourceSave> resourceSaves { get; set; } = new();
    public ReactiveValue<DateTime> timeEnergyDone { get; set; } = new();
    public ReactiveValue<DateTime> timeEnegyAddOne { get; set; } = new();
}
public partial class InGameData
{
    [MemoryPackOrder(105)]
    [field: SerializeField] public PlayerResourceDataSave playerResourceDataSave { get; set; } = new();
}

[System.Serializable]
[MemoryPackable]
public partial class ResourceSave {
    public ResourceType type;
    public double coefficient;
    public int exp;
}
