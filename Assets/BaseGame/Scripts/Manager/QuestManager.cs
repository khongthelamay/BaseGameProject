using Sirenix.OdinInspector;
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
        if (streakDailySaves.Count == 0)
            InitDailyStreak();

        streakWeeklySaves = InGameDataManager.Instance.InGameData.QuestData.streakWeeklySaves;
        if (streakWeeklySaves.Count == 0)
            InitWeeklyStreak();

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
                if (timeDailyRemaining.Value > 0)
                    timeDailyRemaining.Value -= Time.deltaTime;
                else {
                    canCountDailyDown = false;
                    CheckDailyDay();
                } 
                    
            }

            if (canCountWeeklyDown)
            {
                if (timeWeeklyRemaining.Value > 0)
                    timeWeeklyRemaining.Value -= Time.deltaTime;
                else
                {
                    canCountWeeklyDown = false;
                    CheckWeeklyDay();
                }
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
            AddQuestProgress(1,1);
        }

        lastDailyDay = DateTime.Parse(strLastDay, TimeUtil.cultureInfor);
        timeDailyEnd = lastDailyDay.AddDays(1);
        TimeSpan timeSpan = DateTime.Now.Subtract(lastDailyDay);
        if (timeSpan.TotalHours >= 24)
        {
            InGameDataManager.Instance.InGameData.QuestData.SaveDailyDay();
            AddQuestProgress(1, 1);
        }
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
        for (int i = streakDailySaves.Count -1 ; i >=0 ; i--)
        {
            if (streakDailySaves[i].Value.streak <= currentDailyStreak.Value)
            {
                streakDailySaves[i].Value.canClaim.Value = true;
                break;
            }
        }
        InGameDataManager.Instance.SaveData();
    }

    [Button]
    public void AddWeeklyStreak(float amount) {
        currentWeeklyStreak.Value += amount;
        for (int i = streakWeeklySaves.Count - 1; i >= 0; i--)
        {
            if (streakWeeklySaves[i].Value.streak <= currentWeeklyStreak.Value)
            {
                streakWeeklySaves[i].Value.canClaim.Value = true;
                break;
            }
        }
        InGameDataManager.Instance.SaveData();
    }

    public ReactiveValue<StreakSave> GetStreakSave(float streak)
    {
        for (int i = 0; i < streakDailySaves.Count; i++)
        {
            if (streakDailySaves[i].Value.streak == streak)
                return streakDailySaves[i];
        }

        for (int i = 0; i < streakWeeklySaves.Count; i++)
        {
            if (streakWeeklySaves[i].Value.streak == streak)
                return streakWeeklySaves[i];
        }
        return null;
    }

    void InitDailyStreak() {
        for (int i = 0; i < QuestGlobalConfig.Instance.dailyStreaks.Count; i++)
        {
            StreakSave newStreak = new();
            newStreak.streak = QuestGlobalConfig.Instance.dailyStreaks[i].streak;
            newStreak.canClaim.Value = false;
            newStreak.claimed.Value = false;

            ReactiveValue<StreakSave> newStreakSave = new(newStreak);

            streakDailySaves.Add(newStreakSave);
        }
        InGameDataManager.Instance.SaveData();
    }
    void InitWeeklyStreak()
    {
        for (int i = 0; i < QuestGlobalConfig.Instance.weeklyStreaks.Count; i++)
        {
            StreakSave newStreak = new();
            newStreak.streak = QuestGlobalConfig.Instance.weeklyStreaks[i].streak;
            newStreak.canClaim.Value = false;
            newStreak.claimed.Value = false;

            ReactiveValue<StreakSave> newStreakSave = new(newStreak);

            streakWeeklySaves.Add(newStreakSave);
        }
        InGameDataManager.Instance.SaveData();
    }

    public void AddQuestProgress(int questID, int progress) {
        ReactiveValue<QuestSave> questSave = GetQuestSaveData(questID);
        questSave.Value.progress.Value += progress;
    }

    public bool CheckQuestDone() {
        for (int i = 0; i < questSaves.Count; i++)
        {
            if (!questSaves[i].Value.claimed.Value)
            {
                if (QuestGlobalConfig.Instance.CheckQuestDone(questSaves[i].Value))
                    return true;
            }
           
        }
        return false;
    }

    bool IsHaveStreakCanClaim() {
        for (int i = 0; i < streakDailySaves.Count; i++)
        {
            if (streakDailySaves[i].Value.canClaim.Value && !streakDailySaves[i].Value.claimed.Value)
                return true;
        }

        for (int i = 0; i < streakWeeklySaves.Count; i++)
        {
            if (streakWeeklySaves[i].Value.canClaim.Value && !streakWeeklySaves[i].Value.claimed.Value)
                return true;
        }

        return false;
    }

    public void CheckShowNoticeQuest() {
        if (CheckQuestDone())
        {
            Debug.Log("Is have quest done");
            ScreensBattleContext.Events.ShowNoticeQuest?.Invoke(true);
            return;
        }

        if (IsHaveStreakCanClaim())
        {
            Debug.Log("Is have streak done");
            ScreensBattleContext.Events.ShowNoticeQuest?.Invoke(true);
            return;
        }

        ScreensBattleContext.Events.ShowNoticeQuest?.Invoke(false);
    }

    public void ClaimDailyStreak(float streak)
    {
        for (int i = 0; i < streakDailySaves.Count; i++)
        {
            if (streakDailySaves[i].Value.streak == streak)
            {
                streakDailySaves[i].Value.claimed.Value = true;
                InGameDataManager.Instance.SaveData();
                return;
            }
        }
    }

    public void ClaimWeeklyStreak(float streak)
    {
        for (int i = 0; i < streakWeeklySaves.Count; i++)
        {
            if (streakWeeklySaves[i].Value.streak == streak)
            {
                streakWeeklySaves[i].Value.claimed.Value = true;
                InGameDataManager.Instance.SaveData();
                return;
            }
        }
    }
}
