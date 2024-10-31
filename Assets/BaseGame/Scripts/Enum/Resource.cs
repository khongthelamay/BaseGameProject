using MemoryPack;
using System.Collections;
using System.Collections.Generic;
using TW.Reactive.CustomComponent;
using TW.Utility.CustomType;
using Unity.VisualScripting;
using UnityEngine;

public enum ResourceType { 
    Coin = 0,
    Gem = 1,
    SummonRecipe = 2,
    EpicStone = 3,
    Key = 4,
    Energy = 5
}

[System.Serializable]
[MemoryPackable]
public partial class Resource
{
    public ResourceType type;
    public ReactiveValue<BigNumber> value = new(0);

    public void Add(BigNumber amount) { value.Value += amount; }

    public void Consume(BigNumber amount) { value.Value -= amount; }
}


