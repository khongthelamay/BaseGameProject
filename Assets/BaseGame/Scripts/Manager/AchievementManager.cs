using System.Collections;
using System.Collections.Generic;
using TW.Reactive.CustomComponent;
using TW.Utility.DesignPattern;
using UnityEngine;

public class AchievementManager : Singleton<AchievementManager>
{
    public List<ReactiveValue<AchievementSave>> achievements = new();

    private void Start()
    {
        LoadData();
    }

    void LoadData() {
        achievements = InGameDataManager.Instance.InGameData.achievementDataSave.achievements;
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

    bool IsCanUpdateLevelAchievement(AchievementType achievementType, int currentLevel) {
        return AchievementGlobalConfig.Instance.IsCanUpdateLevel(achievementType, currentLevel);
    }
}
