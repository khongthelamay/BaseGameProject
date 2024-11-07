using UnityEngine;
using Sirenix.Utilities;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TW.Utility.Extension;
using System;
using UnityEditor;

[CreateAssetMenu(fileName = "HunterPassGlobalConfig", menuName = "GlobalConfigs/HunterPassGlobalConfig")]
[GlobalConfig("Assets/Resources/GlobalConfig/")]
public class HunterPassGlobalConfig : GlobalConfig<HunterPassGlobalConfig>
{
    public List<HunterPass> hunterPasses = new();

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
        hunterPasses.Clear();

        requestedData = await ABakingSheet.GetCsv(linkSheetId, "HunterPass");

        List<Dictionary<string, string>> data = ACsvReader.ReadDataFromString(requestedData);
        for (int i = 0; i < data.Count; i++)
        {
            int level = int.Parse(data[i]["Level"]);
            int expRequire = int.Parse(data[i]["Exp"]);
            ResourceType rType = (ResourceType) Enum.Parse(typeof(ResourceType), data[i]["RewardType"]);
            int commonRewardAmount = int.Parse(data[i]["CommonRewardAmount"]);
            int premiumRewardAmount = int.Parse(data[i]["PremiumRewardAmount"]);

            HunterPass hunterPass = new();
            hunterPass.level = level;
            hunterPass.expRequire = expRequire;
            hunterPass.rType = rType;
            hunterPass.commonRewardAmount = commonRewardAmount;
            hunterPass.premiumRewardAmount = premiumRewardAmount;

            hunterPasses.Add(hunterPass);
        }

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }
#endif
}

[System.Serializable]
public class HunterPass {
    public int level;
    public int expRequire;
    public ResourceType rType;
    public int commonRewardAmount;
    public int premiumRewardAmount;
}