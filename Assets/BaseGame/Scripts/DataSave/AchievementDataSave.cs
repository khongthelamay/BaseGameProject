using MemoryPack;
using System.Collections;
using System.Collections.Generic;
using TW.Reactive.CustomComponent;
using UnityEngine;

[System.Serializable]
[MemoryPackable]
public class AchievementDataSave : MonoBehaviour
{
    public List<ReactiveValue<AchievementSave>> achievements = new();
}

public partial class InGameData
{
    [field: SerializeField] public AchievementDataSave achievementDataSave { get; set; } = new();
}

[System.Serializable]
[MemoryPackable]
public partial class AchievementSave
{
    [field: SerializeField] public ReactiveValue<int> achievementType { get; set; } = new(0);
    [field: SerializeField] public ReactiveValue<int> achievementLevel { get; set; } = new(0);
    [field: SerializeField] public ReactiveValue<float> currentProgress { get; set; } = new(0);
}
