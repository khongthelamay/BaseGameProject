using System.Collections;
using System.Collections.Generic;
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
public class Resource
{
    public ResourceType type;
    public BigNumber value;

    public void Add(BigNumber amount) { value += amount; }

    public void Consume(BigNumber amount) { value -= amount; }
}


