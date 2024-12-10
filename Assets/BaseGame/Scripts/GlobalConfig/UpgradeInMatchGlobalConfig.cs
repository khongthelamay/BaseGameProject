using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "UpgradeInMatchGlobalConfig", menuName = "GlobalConfigs/UpgradeInMatchGlobalConfig")]
[GlobalConfig("Assets/Resources/GlobalConfig/")]
public class UpgradeInMatchGlobalConfig : GlobalConfig<UpgradeInMatchGlobalConfig>
{
    public List<UpgradeInMatchData> upgradeRareInMatchDatas = new();
    public List<UpgradeInMatchData> upgradeEpicInMatchDatas = new();
    public List<UpgradeInMatchData> upgradeLegendaryInMatchDatas = new();
    public List<UpgradeInMatchData> upgradePersentInMatchDatas = new();

    public UpgradeInMatchData GetUpgradeInMatchData(int level, UpgradeInMatchType upgradeInMatchType) {
        switch (upgradeInMatchType)
        {
            case UpgradeInMatchType.Rare:
                if (level< upgradeRareInMatchDatas.Count)
                    return upgradeRareInMatchDatas[level];
                return null;
            case UpgradeInMatchType.Epic:
                if (level < upgradeEpicInMatchDatas.Count)
                    return upgradeEpicInMatchDatas[level];
                return null;
            case UpgradeInMatchType.Legendary:
                if (level < upgradeLegendaryInMatchDatas.Count)
                    return upgradeLegendaryInMatchDatas[level];
                return null;
            case UpgradeInMatchType.Persent:
                if (level < upgradePersentInMatchDatas.Count)
                    return upgradePersentInMatchDatas[level];
                return null;
            default:
                return null;
        }
    }
}

[System.Serializable]
public class UpgradeInMatchData
{
    public int level;
    public Resource resource;
}
