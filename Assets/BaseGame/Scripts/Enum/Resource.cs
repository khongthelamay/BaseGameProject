using MemoryPack;
using System;
using TW.Reactive.CustomComponent;
using TW.Utility.CustomType;

public enum ResourceType { 
    None = 0,
    Coin = 1,
    Gem = 2,
    SummonRecipe = 3,
    EpicStone = 4,
    Key = 5,
    Energy = 6,
    RecruitRecipe = 7
}

[System.Serializable]
[MemoryPackable]
public partial class Resource
{
    public ResourceType type;
    public ReactiveValue<BigNumber> value = new(0);

    public void Add(BigNumber amount) { value.Value += amount; }

    public void Consume(BigNumber amount) { value.Value -= amount; }

    public void ChangeValue(BigNumber amount) { value.Value += amount; }
}


