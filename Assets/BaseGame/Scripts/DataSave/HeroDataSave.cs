using MemoryPack;
using System.Collections.Generic;
using TW.Reactive.CustomComponent;
using UnityEngine;

[System.Serializable]
[MemoryPackable]
public partial class HeroDataSave
{
    public static HeroDataSave Instance => InGameDataManager.Instance.InGameData.heroDataSave;
    [field: SerializeField] public List<ReactiveValue<HeroSave>> heroSaves { get; set; } = new();
    [field: SerializeField] public ReactiveValue<int> totalMysthicUpgradePieces { get; set; } = new(0);
}
public partial class InGameData
{
    [field: SerializeField] public HeroDataSave heroDataSave { get; set; } = new();
}

[System.Serializable]
[MemoryPackable]
public partial class HeroSave {
    public string heroName;
    public ReactiveValue<int> level;
    public ReactiveValue<int> piece;

    public void UpgradeHero() { level.Value++; }
}

