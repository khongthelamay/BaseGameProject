using System;
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

    public ReactiveValue<float> timeDailyRemaining = new();
    public ReactiveValue<float> timeWeeklyRemaining = new();

    DateTime lastDailyDay;
    DateTime lastWeeklyDay;
    public DateTime timeDailyEnd;
    DateTime timeWeelyEnd;

    bool canCountDailyDown;
    bool canCountWeeklyDown;
    public bool showModalQuest;
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
    
    private void FixedUpdate()
    {
        if (showModalQuest)
        {
            if (canCountDailyDown)
            {
                timeDailyRemaining.Value -= Time.deltaTime;
                //ModalQuestContext.Events.ChangeTextDaily?.Invoke();
                //if (timeDailyRemaining.Value <= 0)
                //{
                //    //strTimeDailyRemaining.Value = "0";
                //    canCountDailyDown = false;
                //    ChangeDayOnGame();
                //}
                //else strTimeDailyRemaining.Value = TimeUtil.TimeToString(timeDailyRemaining, TimeFommat.Keyword);
            }

            if (canCountWeeklyDown)
            {
                timeWeeklyRemaining.Value -= Time.deltaTime;
                //if (timeWeeklyRemaining.Value <= 0)
                //{
                //    //strTimeWeeklyRemaining.Value = "0";
                //    canCountWeeklyDown = false;
                //    ChangeWeekOnGame();
                //}
                //else strTimeWeeklyRemaining.Value = TimeUtil.TimeToString(timeWeeklyRemaining, TimeFommat.Keyword);
            }
        }
    }

    public void ShowModelQuest() {
        timeDailyRemaining.Value = (float)timeDailyEnd.Subtract(DateTime.Now).TotalSeconds;

        timeWeeklyRemaining.Value = (float)timeWeelyEnd.Subtract(DateTime.Now).TotalSeconds;
        showModalQuest = true;
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

    void ChangeWeekOnGame() {
        InGameDataManager.Instance.InGameData.QuestData.SaveWeeklyDay();
        CheckWeeklyDay();
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
        float starReward = QuestGlobalConfig.Instance.GetStarAmount(questID);
        AddDailyStreak(starReward);
        AddWeeklyStreak(starReward);
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

    public bool CheckStreakDone(float streak)
    {
        for (int i = 0; i < streakDailySaves.Count; i++)
        {
            if (streakDailySaves[i].Value.streak == streak)
                return true;
        }
        return false;
    }

    public ReactiveValue<StreakSave> GetStreakSave(float streak)
    {
        for (int i = 0; i < streakDailySaves.Count; i++)
        {
            if (streakDailySaves[i].Value.streak == streak)
                return streakDailySaves[i];
        }

        StreakSave newStreak = new();
        newStreak.streak = streak;
        newStreak.canClaim.Value = false;
        newStreak.claimed.Value = false;

        ReactiveValue<StreakSave> newStreakSave = new(newStreak);

        streakDailySaves.Add(newStreakSave);

        return newStreakSave;
    }
}
