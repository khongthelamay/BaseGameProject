using UnityEngine;
using Sirenix.Utilities;
using TW.Utility.CustomType;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using System;
using TW.Utility.Extension;
using UnityEditor;

[CreateAssetMenu(fileName = "GrowGlobalConfig", menuName = "GlobalConfigs/GrowGlobalConfig")]
[GlobalConfig("Assets/Resources/GlobalConfig/")]
public class GrowGlobalConfig : GlobalConfig<GrowGlobalConfig>
{
    public List<GrowthDataConfig> growGlobalConfigs = new();

#if UNITY_EDITOR
    string linkSheetId = "1-HkinUwSW4A4SkuiLGtl0Tm8771jFPVZB5ZpLs5pxz4";

    string requestedData;

    [Button]
    public void FetchGrowthData()
    {
        if (string.IsNullOrEmpty(linkSheetId)) return;
        FetchGrowth();
    }
    async void FetchGrowth()
    {
        growGlobalConfigs.Clear();

        requestedData = await ABakingSheet.GetCsv(linkSheetId, "HunterPass");

        List<Dictionary<string, string>> data = ACsvReader.ReadDataFromString(requestedData);
        for (int i = 0; i < data.Count; i++)
        {
            int level = int.Parse(data[i]["Level"].ToString());
            float expRequire = int.Parse(data[i]["Exp"].ToString());
            ResourceType rtype = (ResourceType) Enum.Parse(typeof(ResourceType), data[i]["RewardType"]);
            BigNumber commondRewardAmount = new BigNumber(data[i]["CommonRewardAmount"].ToString());
            BigNumber premiumRewardAmount = new BigNumber(data[i]["PremiumRewardAmount"].ToString());

            GrowthDataConfig newData = new();
            newData.level = level;
            newData.expRequire = expRequire;
            newData.rType = rtype;
            newData.commonRewardAmount = commondRewardAmount;
            newData.premiumRewardAmount = premiumRewardAmount;

            growGlobalConfigs.Add(newData);

        }
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }
#endif
}

[System.Serializable]
public class GrowthDataConfig
{
    public int level;
    public float expRequire;
    public ResourceType rType;
    public BigNumber commonRewardAmount;
    public BigNumber premiumRewardAmount;
}