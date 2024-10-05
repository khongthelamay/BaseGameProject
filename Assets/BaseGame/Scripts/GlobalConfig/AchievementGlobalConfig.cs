using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using TW.Utility.CustomType;
using TW.Utility.Extension;
using UnityEngine;

[CreateAssetMenu(fileName = "AchievementGlobalConfig", menuName = "GlobalConfigs/AchievementGlobalConfig")]
[GlobalConfig("Assets/Resources/GlobalConfig/")]
public class AchievementGlobalConfig : GlobalConfig<AchievementGlobalConfig>
{
    public List<AchievementDataConfig> achievementDataConfigs = new();
    public List<AchievementDataConfig> achievementKillEnemyDataConfigs = new();
    public List<AchievementDataConfig> achievementKillBossDataConfigs = new();
    public List<AchievementDataConfig> achievementClearQuestDataConfigs = new();
    public List<AchievementDataConfig> achievementClearWaveNormalDataConfigs = new();
    public List<AchievementDataConfig> achievementClearWaveHardDataConfigs = new();
    public List<AchievementDataConfig> achievementClearWaveHellDataConfigs = new();
    public List<AchievementDataConfig> achievementRecruitDataConfigs = new();
    public List<AchievementDataConfig> achievementUpgradeUnitDataConfigs = new();
    public List<AchievementDataConfig> achievementSummonTier3DataConfigs = new();
    public List<AchievementDataConfig> achievementSummonTier4DataConfigs = new();
    public List<AchievementDataConfig> achievementSummonTier5DataConfigs = new();

    public bool IsCanUpdateLevel(AchievementType achievementType, int currentLevel)
    {
        switch (achievementType)
        {
            case AchievementType.KillEnemy:
                return achievementKillEnemyDataConfigs.Count > currentLevel;

            case AchievementType.KillBoss:
                return achievementKillBossDataConfigs.Count > currentLevel;

            case AchievementType.ClearQuest:
                return achievementClearQuestDataConfigs.Count > currentLevel;

            case AchievementType.ClearWaveNormal:
                return achievementClearWaveNormalDataConfigs.Count > currentLevel;

            case AchievementType.ClearWaveHard:
                return achievementClearWaveHardDataConfigs.Count > currentLevel;

            case AchievementType.ClearWaveHell:
                return achievementClearWaveHellDataConfigs.Count > currentLevel;

            case AchievementType.Recruit:
                return achievementRecruitDataConfigs.Count > currentLevel;

            case AchievementType.UpgradeUnit:
                return achievementUpgradeUnitDataConfigs.Count > currentLevel;

            case AchievementType.SummonTier3:
                return achievementSummonTier3DataConfigs.Count > currentLevel;

            case AchievementType.SummonTier4:
                return achievementSummonTier4DataConfigs.Count > currentLevel;

            case AchievementType.SummonTier5:
                return achievementSummonTier5DataConfigs.Count > currentLevel;

            default:
                return false;
        }
    }

#if UNITY_EDITOR
    string linkSheetId = "1-HkinUwSW4A4SkuiLGtl0Tm8771jFPVZB5ZpLs5pxz4";
    string requestedData;

    [Button]
    public void FetchQuestData()
    {
        if (string.IsNullOrEmpty(linkSheetId)) return;
        FetchAchievement();
    }

    async void FetchAchievement()
    {
        achievementDataConfigs.Clear();

        requestedData = await ABakingSheet.GetCsv(linkSheetId, "Achievement");

        List<Dictionary<string, string>> data = ACsvReader.ReadDataFromString(requestedData);

        achievementKillEnemyDataConfigs.Clear();
        achievementKillBossDataConfigs.Clear();
        achievementClearQuestDataConfigs.Clear();
        achievementClearWaveNormalDataConfigs.Clear();
        achievementClearWaveHardDataConfigs.Clear();
        achievementClearWaveHellDataConfigs.Clear();
        achievementRecruitDataConfigs.Clear();
        achievementUpgradeUnitDataConfigs.Clear();
        achievementSummonTier3DataConfigs.Clear();
        achievementSummonTier4DataConfigs.Clear();
        achievementSummonTier5DataConfigs.Clear();

        for (int i = 0; i < data.Count; i++)
        {

            if (!string.IsNullOrEmpty(data[i]["KillEnemy"]))
                achievementKillEnemyDataConfigs.Add(AddAchievement(data[i]["KillEnemy"], AchievementType.KillEnemy));

            if (!string.IsNullOrEmpty(data[i]["KillBoss"]))
                achievementKillBossDataConfigs.Add(AddAchievement(data[i]["KillBoss"], AchievementType.KillBoss));

            if (!string.IsNullOrEmpty(data[i]["ClearQuest"]))
                achievementClearQuestDataConfigs.Add(AddAchievement(data[i]["ClearQuest"], AchievementType.ClearQuest));

            if (!string.IsNullOrEmpty(data[i]["ClearWaveNormal"]))
                achievementClearWaveNormalDataConfigs.Add(AddAchievement(data[i]["ClearWaveNormal"], AchievementType.ClearWaveNormal));

            if (!string.IsNullOrEmpty(data[i]["ClearWaveHard"]))
                achievementClearWaveHardDataConfigs.Add(AddAchievement(data[i]["ClearWaveHard"], AchievementType.ClearWaveHard));

            if (!string.IsNullOrEmpty(data[i]["ClearWaveHell"]))
                achievementClearWaveHellDataConfigs.Add(AddAchievement(data[i]["ClearWaveHell"], AchievementType.ClearWaveHell));

            if (!string.IsNullOrEmpty(data[i]["Recruit"]))
                achievementRecruitDataConfigs.Add(AddAchievement(data[i]["Recruit"], AchievementType.Recruit));

            if (!string.IsNullOrEmpty(data[i]["UpgradeUnit"]))
                achievementUpgradeUnitDataConfigs.Add(AddAchievement(data[i]["UpgradeUnit"], AchievementType.UpgradeUnit));

            if (!string.IsNullOrEmpty(data[i]["SummonTier3"]))
                achievementSummonTier3DataConfigs.Add(AddAchievement(data[i]["SummonTier3"], AchievementType.SummonTier3));

            if (!string.IsNullOrEmpty(data[i]["SummonTier4"]))
                achievementSummonTier4DataConfigs.Add(AddAchievement(data[i]["SummonTier4"], AchievementType.SummonTier4));

            if (!string.IsNullOrEmpty(data[i]["SummonTier5"]))
                achievementSummonTier5DataConfigs.Add(AddAchievement(data[i]["SummonTier5"], AchievementType.SummonTier5));


        }
    }

    private AchievementDataConfig AddAchievement(string progress, AchievementType achievementType)
    {
        AchievementDataConfig newAchievement = new();
        newAchievement.require = float.Parse(progress);
        newAchievement.achievementType = achievementType;
        newAchievement.reward = new();
        newAchievement.strDes = string.Format(GetStringDes(achievementType), newAchievement.require);
        return newAchievement;
    }

    string GetStringDes(AchievementType achievementType) {
        switch (achievementType)
        {
            case AchievementType.KillEnemy:
                return "Kill {0} enemies.";
            case AchievementType.KillBoss:
                return "Kill {0} boss.";
            case AchievementType.ClearQuest:
                return "Clear {0} quests.";
            case AchievementType.ClearWaveNormal:
                return "Clear {0} normal waves.";
            case AchievementType.ClearWaveHard:
                return "Clear {0} hard waves.";
            case AchievementType.ClearWaveHell:
                return "Clear {0} hell waves.";
            case AchievementType.Recruit:
                return "Recruit {0} times.";
            case AchievementType.UpgradeUnit:
                return "Upgrade unit {0} times.";
            case AchievementType.SummonTier3:
                return "Summon unit tier 3 {0} times.";
            case AchievementType.SummonTier4:
                return "Summon unit tier 4 {0} times.";
            case AchievementType.SummonTier5:
                return "Summon unit tier 5 {0} times.";
            default:
                return "";
        }
    }
#endif
}

[System.Serializable]
public class AchievementDataConfig {
    public AchievementType achievementType;
    public string strDes;
    public float require;
    public Resource reward;
}

public enum AchievementType {
    None = 0,
    KillEnemy = 1,
    KillBoss = 2,
    ClearQuest = 3,
    ClearWaveNormal = 4,
    ClearWaveHard = 5,
    ClearWaveHell = 6,
    Recruit = 7,
    UpgradeUnit = 8,
    SummonTier3 = 9,
    SummonTier4 = 10,
    SummonTier5 = 11
}
