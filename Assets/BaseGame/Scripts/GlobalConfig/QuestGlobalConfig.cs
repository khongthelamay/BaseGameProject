using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using TW.Utility.Extension;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestGlobalConfig", menuName = "GlobalConfigs/QuestGlobalConfig")]
[GlobalConfig("Assets/Resources/GlobalConfig/")]
public class QuestGlobalConfig : GlobalConfig<QuestGlobalConfig>
{
    public List<QuestDataConfig> questDataConfigs = new();
    public List<QuestStreak> questStreaks = new();
    public List<QuestStreak> weeklyStreaks = new();


#if UNITY_EDITOR
    string linkSheetId = "1s-SkzJCXdQbusszYdOaXpToyNJqlQsdZlKI5efmz4L4";
    string requestedData;

    [Button]
    public void FetchQuestData()
    {
        if (string.IsNullOrEmpty(linkSheetId)) return;
        FetchQuest();
        FetchDailyStreak();
        FetchWeeklyStreak();
    }

    async void FetchQuest() {
        questDataConfigs.Clear();

        requestedData = await ABakingSheet.GetCsv(linkSheetId, "Quest");

        List<Dictionary<string, string>> data = ACsvReader.ReadDataFromString(requestedData);

        for (int i = 0; i < data.Count; i++)
        {
            int questID = int.Parse(data[i]["ID"]);
            string questDes = data[i]["QuestDescription"];
            int questReward = int.Parse(data[i]["Star"]);
            int questRequire = int.Parse(data[i]["Require"]);

            QuestDataConfig newQuest = new();
            newQuest.questID = questID;
            newQuest.questDes = questDes;
            newQuest.starReward = questReward;
            newQuest.questRequire = questRequire;

            questDataConfigs.Add(newQuest);
        }
    }

    async void FetchDailyStreak() {
        questStreaks.Clear();
        requestedData = await ABakingSheet.GetCsv(linkSheetId, "Quest");

        List<Dictionary<string, string>> data = ACsvReader.ReadDataFromString(requestedData);

        for (int i = 0; i < data.Count; i++)
        {
            if (!string.IsNullOrEmpty(data[i]["Daily_Streak"]))
            {
                float streak = float.Parse(data[i]["Daily_Streak"]);
                int gold = int.Parse(data[i]["Daily_Gold"]);
                int rewardAmount = int.Parse(data[i]["Daily_Amount"]);
                ResourceType resourceType = (ResourceType)Enum.Parse(typeof(ResourceType), data[i]["Daily_RewardType"]);

                Resource reward = new();
                reward.type = resourceType;
                reward.Add(rewardAmount);

                QuestStreak newStreak = new();
                newStreak.streak = streak;
                newStreak.gold = gold;
                newStreak.reward = reward;

                questStreaks.Add(newStreak);
            }
        }
    }

    async void FetchWeeklyStreak()
    {
        weeklyStreaks.Clear();
        requestedData = await ABakingSheet.GetCsv(linkSheetId, "Quest");

        List<Dictionary<string, string>> data = ACsvReader.ReadDataFromString(requestedData);

        for (int i = 0; i < data.Count; i++)
        {
            if (!string.IsNullOrEmpty(data[i]["Weekly_Streak"]))
            {
                int streak = int.Parse(data[i]["Weekly_Streak"]);
                int rewardAmout = int.Parse(data[i]["Weekly_Amount"]);
                ResourceType resourceType = (ResourceType)Enum.Parse(typeof(ResourceType), data[i]["Weekly_RewardType"]);

                Resource reward = new();
                reward.type = resourceType;
                reward.Add(rewardAmout);

                QuestStreak newStreak = new();
                newStreak.streak = streak;
                newStreak.reward = reward;

                weeklyStreaks.Add(newStreak);
            }
        }
    }
#endif

    public bool IsDoneProgress(QuestSave questSave)
    {
        for (int i = 0; i < questDataConfigs.Count; i++)
        {
            if (questDataConfigs[i].questID == questSave.id.Value)
            {
                if (questDataConfigs[i].questRequire <= questSave.progress.Value)
                    return true;
                else
                    return false;

            }
        }
        return false;
    }
}

[System.Serializable]
public class QuestDataConfig {
    public int questID;
    public string questDes;
    public int starReward;
    public int questRequire;
}

[System.Serializable]
public class QuestStreak {
    public float streak;
    public int gold;
    public Resource reward;
}
