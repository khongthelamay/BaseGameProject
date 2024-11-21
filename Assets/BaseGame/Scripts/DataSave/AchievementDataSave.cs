using MemoryPack;
using System.Collections.Generic;
using TW.Reactive.CustomComponent;
using UnityEngine;

[System.Serializable]
[MemoryPackable]
public partial class AchievementDataSave
{
    public static AchievementDataSave Instance => InGameDataManager.Instance.InGameData.achievementDataSave;
    public List<ReactiveValue<AchievementSave>> achievements = new();

    public int GetArchievementLevel(AchievementType achievementType)
    {
        for (int i = 0; i < achievements.Count; i++)
        {
            if ((int)achievementType == achievements[i].Value.achievementType)
                return achievements[i].Value.achievementLevel;
        }
        return 0;
    }
}

public partial class InGameData
{
    [MemoryPackOrder(101)]
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
