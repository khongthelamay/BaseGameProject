using MemoryPack;
using System;
using System.Collections.Generic;
using TW.Reactive.CustomComponent;
using UnityEngine;

[System.Serializable]
[MemoryPackable]
public partial class QuestDataSave
{
    public static QuestDataSave Instance => InGameDataManager.Instance.InGameData.QuestData;
    [field: SerializeField] public List<ReactiveValue<QuestSave>> questSaves { get; set; } = new();
    [field: SerializeField] public List<ReactiveValue<StreakSave>> streakDailySaves { get; set; } = new();
    [field: SerializeField] public List<ReactiveValue<StreakSave>> streakWeeklySaves { get; set; } = new();

    [field: SerializeField] public ReactiveValue<string> strLastDay { get; set; } = new("");
    [field: SerializeField] public ReactiveValue<string> strLastWeek { get; set; } = new("");

    [field: SerializeField] public ReactiveValue<float> currentDailyStreak { get; set; } = new();
    [field: SerializeField] public ReactiveValue<float> currentWeeklyStreak { get; set; } = new();

    public void SaveDailyDay()
    {
        strLastDay.Value = DateTime.Now.ToString(TimeUtil.GetCultureInfo());

        for (int i = 0; i < questSaves.Count; i++)
        {
            questSaves[i].Value.ResetQuest();
        }

        for (int i = 0; i < streakDailySaves.Count; i++)
        {
            streakDailySaves[i].Value.ResetStreak();
        }
        currentDailyStreak.Value = 0;
        InGameDataManager.Instance.SaveData();    
    }

    public void SaveWeeklyDay()
    {
        strLastWeek.Value = DateTime.Now.ToString(TimeUtil.GetCultureInfo());
        for (int i = 0; i < streakWeeklySaves.Count; i++)
        {
            streakWeeklySaves[i].Value.ResetStreak();
        }
        currentWeeklyStreak.Value = 0;
        InGameDataManager.Instance.SaveData();
    }
}
public partial class InGameData
{
    [MemoryPackOrder(106)]
    [field: SerializeField] public QuestDataSave QuestData { get; set; } = new();
}

[System.Serializable]
[MemoryPackable]
public partial class QuestSave
{
    [field: SerializeField] public ReactiveValue<int> id { get; set; } = new(0);
    [field: SerializeField] public ReactiveValue<int> progress { get; set; } = new(0);
    [field: SerializeField] public ReactiveValue<bool> claimed { get; set; } = new(false);

    public void ResetQuest()
    {
        progress.Value = 0;
        claimed.Value = false;
    }
}

[System.Serializable]
[MemoryPackable]
public partial class StreakSave
{
    [field: SerializeField] public float streak { get; set; }
    [field: SerializeField] public ReactiveValue<bool> canClaim { get; set; } = new(false);
    [field: SerializeField] public ReactiveValue<bool> claimed { get; set; } = new(false);

    public void ResetStreak() {
        canClaim.Value = false;
        claimed.Value = false;
    }
}

