using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TW.Utility.Extension;
using Sirenix.Utilities;
using TW.Utility.CustomType;

[CreateAssetMenu(fileName = "RoundRewardGlobalConfig", menuName = "GlobalConfigs/RoundRewardGlobalConfig")]
[GlobalConfig("Assets/Resources/GlobalConfig/")]
public class RoundRewardGlobalConfig :GlobalConfig<RoundRewardGlobalConfig>
{
    public List<RoundRewardConfig> roundRewardConfigs = new();

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
        requestedData = await ABakingSheet.GetCsv(linkSheetId, "Gacha");

        List<Dictionary<string, string>> data = ACsvReader.ReadDataFromString(requestedData);
    }
#endif
}

[System.Serializable]
public class RoundRewardConfig {
    public int level;
    public ResourceType rewardType;
    public BigNumber amount;
}
