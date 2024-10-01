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
    public ReactiveValue<string> strTimeWeeklyRemaining;

    float timeDailyRemaining;
    float timeWeeklyRemaining;

    DateTime lastDailyDay;
    DateTime lastWeeklyDay;
    DateTime timeDailyEnd;
    DateTime timeWeelyEnd;

    bool canCountDailyDown;
    bool canCountWeeklyDown;
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

        CheckDailyDay();
        CheckWeeklyDay();

    }

    private void Update()
    {
        if (canCountDailyDown)
        {
            timeDailyRemaining = (float)timeDailyEnd.Subtract(DateTime.Now).TotalSeconds;

            if (timeDailyRemaining <= 0)
            {
                strTimeDailyRemaining.Value = "0";
                canCountDailyDown = false;
                ChangeDayOnGame();
            }
            else strTimeDailyRemaining.Value = TimeUtil.TimeToString(timeDailyRemaining, TimeFommat.Keyword);
        }

        if (canCountWeeklyDown)
        {
            timeWeeklyRemaining = (float)timeWeelyEnd.Subtract(DateTime.Now).TotalSeconds;
            if (timeWeeklyRemaining <= 0)
            {
                strTimeWeeklyRemaining.Value = "0";
                canCountWeeklyDown = false;
                ChangeDayOnGame();
            }
            else strTimeWeeklyRemaining.Value = TimeUtil.TimeToString(timeWeeklyRemaining, TimeFommat.Keyword);
        }
    }

    void CheckDailyDay() {
        if (string.IsNullOrEmpty(strLastDay))
        {
            InGameDataManager.Instance.InGameData.QuestData.SaveDailyDay();
        }

        lastDailyDay = DateTime.Parse(strLastDay, TimeUtil.cultureInfor);
        timeDailyEnd = lastDailyDay.AddDays(1);
        TimeSpan timeSpan = DateTime.Now.Subtract(lastDailyDay);
        if (timeSpan.TotalHours >= 24)
            InGameDataManager.Instance.InGameData.QuestData.SaveDailyDay();

        canCountDailyDown = true;
    }

    void CheckWeeklyDay() {
        if (string.IsNullOrEmpty(strLastWeek))
        {
            InGameDataManager.Instance.InGameData.QuestData.SaveWeeklyDay();
        }

        lastWeeklyDay = DateTime.Parse(strLastWeek, TimeUtil.cultureInfor);
        timeWeelyEnd = lastWeeklyDay.AddDays(7);
        TimeSpan timeSpan = DateTime.Now.Subtract(lastWeeklyDay);
        if (timeSpan.TotalDays >= 7)
            InGameDataManager.Instance.InGameData.QuestData.SaveWeeklyDay();
        
        canCountWeeklyDown = true;
    }

    void ChangeDayOnGame() {
        InGameDataManager.Instance.InGameData.QuestData.SaveDailyDay();
        CheckDailyDay();
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
