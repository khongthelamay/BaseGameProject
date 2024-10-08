using System;
using System.Collections;
using System.Collections.Generic;
using TW.Reactive.CustomComponent;
using TW.Utility.DesignPattern;
using UnityEngine;

public class AchievementManager : Singleton<AchievementManager>
{
    public List<ReactiveValue<AchievementSave>> achievements = new();

   

    public List<AchievementDataConfig> achievementDataConfigs = new();

    private void Start()
    {
        LoadData();
    }

    void LoadData() {
        achievements = InGameDataManager.Instance.InGameData.achievementDataSave.achievements;
    }

    public AchievementSave GetAchievementDataSave(AchievementType achievementType)
    {
        for (int i = 0; i < achievements.Count; i++)
        {
            if (achievements[i].Value.achievementType == (int)achievementType)
                return achievements[i].Value;
        }
        AddAchievement(achievementType);
        return achievements[achievements.Count - 1];
    }

    void AddAchievement(AchievementType achievementType) {
        AchievementSave newAchievement = new();
        newAchievement.achievementType.Value = (int)achievementType;
        newAchievement.currentProgress.Value = 0;
        ReactiveValue<AchievementSave> newRAchievement = new();
        newRAchievement.Value = newAchievement;
        achievements.Add(newRAchievement);
        InGameDataManager.Instance.SaveData();
    }

    public void AddProgressAchievement(AchievementType achievementType, float amount) {
        for (int i = 0; i < achievements.Count; i++)
        {
            if (achievements[i].Value.achievementType == (int)achievementType)
            {
                achievements[i].Value.currentProgress.Value += amount;
                return;
            }
        }
    }

    public bool IsCanClaim(AchievementSave achievement)
    {
        for (int i = 0; i < achievementDataConfigs.Count; i++)
        {
            if (achievementDataConfigs[i].achievementType == (AchievementType)achievement.achievementType.Value)
            {
                return achievementDataConfigs[i].require <= achievement.currentProgress.Value;
            }
        }
        return false;
    }

    public void ClaimAchievement(AchievementType achievementType) {
        for (int i = 0; i < achievements.Count; i++)
        {
            if (achievements[i].Value.achievementType == (int)achievementType)
            {
                if(IsCanUpdateLevelAchievement(achievementType, achievements[i].Value.achievementLevel.Value))
                    achievements[i].Value.achievementLevel.Value += 1;
                return;
            }
        }
    }

    public bool IsCanUpdateLevelAchievement(AchievementType achievementType, int currentLevel) {
        return AchievementGlobalConfig.Instance.IsCanUpdateLevel(achievementType, currentLevel);
    }

    public List<AchievementDataConfig> GetAchievements() {
        achievementDataConfigs.Clear();
        for (int i = 1; i < Enum.GetNames(typeof(AchievementType)).Length; i++)
        {
            achievementDataConfigs.Add(
                AchievementGlobalConfig.Instance.GetCurrentAchievement(
                    (AchievementType)i, 
                    InGameDataManager.Instance.InGameData.achievementDataSave.GetArchievementLevel((AchievementType)i)
                )
            );
        }
        return achievementDataConfigs;
    }
}
