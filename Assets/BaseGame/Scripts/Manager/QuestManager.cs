using System;
using System.Collections;
using System.Collections.Generic;
using TW.Reactive.CustomComponent;
using TW.Utility.DesignPattern;
using UnityEngine;

public class QuestManager : Singleton<QuestManager>
{
    [field: SerializeField] public List<ReactiveValue<QuestSave>> questSaves { get; set; } = new();

    [field: SerializeField] public List<ReactiveValue<StreakSave>> streakDailySaves { get; set; } = new();
    [field: SerializeField] public List<ReactiveValue<StreakSave>> streakWeeklySaves { get; set; } = new();

    [field: SerializeField] public ReactiveValue<string> strLastDay { get; set; } = new();
    [field: SerializeField] public ReactiveValue<string> strLastWeek { get; set; } = new();

    [field: SerializeField] public ReactiveValue<float> currentDailyStreak { get; set; } = new();
    [field: SerializeField] public ReactiveValue<float> currentWeeklyStreak { get; set; } = new();


    public ReactiveValue<string> strTimeDailyRemaining;

    float timeDailyRemaining;

    DateTime lastDailyDay;
    DateTime timeDailyEnd;

    bool canCountDown;
    private void Start()
    {
        LoadData();
    }

    private void LoadData()
    {
        questSaves = InGameDataManager.Instance.InGameData.QuestData.questSaves;
        streakDailySaves = InGameDataManager.Instance.InGameData.QuestData.streakDailySaves;
        streakWeeklySaves = InGameDataManager.Instance.InGameData.QuestData.streakWeeklySaves;

        strLastDay = InGameDataManager.Instance.InGameData.QuestData.strLastDay;
        strLastWeek = InGameDataManager.Instance.InGameData.QuestData.strLastWeek;

        currentDailyStreak = InGameDataManager.Instance.InGameData.QuestData.currentDailyStreak;
        currentWeeklyStreak = InGameDataManager.Instance.InGameData.QuestData.currentWeeklyStreak;

        CheckDay();

    }

    private void Update()
    {
        if (canCountDown)
        {
            timeDailyRemaining = (float)timeDailyEnd.Subtract(DateTime.Now).TotalSeconds;

            if (timeDailyRemaining <= 0)
            {
                strTimeDailyRemaining.Value = "0";
                canCountDown = false;
                ChangeDayOnGame();
            }
            else strTimeDailyRemaining.Value = TimeUtil.TimeToString(timeDailyRemaining, TimeFommat.Keyword);
        }
    }

    void CheckDay() {
        if (string.IsNullOrEmpty(strLastDay))
        {
            InGameDataManager.Instance.InGameData.QuestData.SaveDailyDay();
        }

        lastDailyDay = DateTime.Parse(strLastDay, TimeUtil.cultureInfor);
        timeDailyEnd = lastDailyDay.AddDays(1);
        TimeSpan timeSpan = DateTime.Now.Subtract(lastDailyDay);
        if (timeSpan.TotalHours >= 24)
            InGameDataManager.Instance.InGameData.QuestData.SaveDailyDay();
        canCountDown = true;
    }

    void ChangeDayOnGame() {
        InGameDataManager.Instance.InGameData.QuestData.SaveDailyDay();
        CheckDay();
    }

    public ReactiveValue<QuestSave> GetQuestSaveData(int questID) {
        for (int i = 0; i < questSaves.Count; i++)
        {
            if (questSaves[i].Value.id.Value == questID)
                return questSaves[i];
        }

        QuestSave newQuest = new();
        newQuest.id.Value = questID;
        newQuest.progress.Value = 0;
        newQuest.claimed.Value = false;

        ReactiveValue<QuestSave> newQuestSave = new(newQuest);

        questSaves.Add(newQuestSave);

        return newQuestSave;
    }

    public bool IsCanClaim(int questID) {
        for (int i = 0; i < questSaves.Count; i++)
        {
            if (questSaves[i].Value.id.Value == questID)
            {
                if (questSaves[i].Value.claimed.Value == true)
                    return false;
                return QuestGlobalConfig.Instance.IsDoneProgress(questSaves[i].Value);
            }
        }
        return false;
    }

    public void ClaimQuest(int questID) {
        for (int i = 0; i < questSaves.Count; i++)
        {
            if (questSaves[i].Value.id == questID)
                questSaves[i].Value.claimed.Value = true;
        }
        InGameDataManager.Instance.SaveData();
    }

    public void AddDailyStreak(float amount) {
        currentDailyStreak.Value += amount;
        InGameDataManager.Instance.SaveData();
    }

    public void AddWeeklyStreak(float amount) {
        currentWeeklyStreak.Value += amount;
        InGameDataManager.Instance.SaveData();
    }

    void CheckStreakDone()
    {
        for (int i = 0; i < streakDailySaves.Count; i++)
        {
            
        }
    }
}
