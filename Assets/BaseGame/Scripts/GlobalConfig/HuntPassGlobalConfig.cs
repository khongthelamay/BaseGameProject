using UnityEngine;
using Sirenix.Utilities;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TW.Utility.Extension;
using System;
using UnityEditor;

[CreateAssetMenu(fileName = "HuntPassGlobalConfig", menuName = "GlobalConfigs/HuntPassGlobalConfig")]
[GlobalConfig("Assets/Resources/GlobalConfig/")]
public class HuntPassGlobalConfig : GlobalConfig<HuntPassGlobalConfig>
{
    public List<HuntPass> huntPasses = new();

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
        huntPasses.Clear();

        requestedData = await ABakingSheet.GetCsv(linkSheetId, "HunterPass");

        List<Dictionary<string, string>> data = ACsvReader.ReadDataFromString(requestedData);
        for (int i = 0; i < data.Count; i++)
        {
            int level = int.Parse(data[i]["Level"]);
            int expRequire = int.Parse(data[i]["Exp"]);
            ResourceType rType = (ResourceType) Enum.Parse(typeof(ResourceType), data[i]["RewardType"]);
            int commonRewardAmount = int.Parse(data[i]["CommonRewardAmount"]);
            int premiumRewardAmount = int.Parse(data[i]["PremiumRewardAmount"]);

            HuntPass huntPass = new();
            huntPass.level = level;
            huntPass.expRequire = expRequire;
            huntPass.rType = rType;
            huntPass.commonRewardAmount = commonRewardAmount;
            huntPass.premiumRewardAmount = premiumRewardAmount;

            huntPasses.Add(huntPass);
        }

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }
#endif
}

[System.Serializable]
public class HuntPass {
    public int level;
    public int expRequire;
    public ResourceType rType;
    public int commonRewardAmount;
    public int premiumRewardAmount;
}