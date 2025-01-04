using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
[CreateAssetMenu(fileName = "UpgradeInMatchGlobalConfig", menuName = "GlobalConfigs/UpgradeInMatchGlobalConfig")]
[GlobalConfig("Assets/Resources/GlobalConfig/")]
public class UpgradeInMatchGlobalConfig : GlobalConfig<UpgradeInMatchGlobalConfig>
{
    [field: SerializeField, TableList] 
    public List<UpgradeInMatchData> UpgradeRareConfig { get; set; } = new();
    [field: SerializeField, TableList] 
    public List<UpgradeInMatchData> UpgradeEpicConfig { get; set; } = new();
    [field: SerializeField, TableList] 
    public List<UpgradeInMatchData> UpgradeLegendaryConfig { get; set; } = new();
    [field: SerializeField, TableList] 
    public List<UpgradeInMatchData> UpgradePercentConfig { get; set; } = new();

    public UpgradeInMatchData GetUpgradeInMatchData(int level, UpgradeInMatchType upgradeInMatchType)
    {
        return upgradeInMatchType switch
        {
            UpgradeInMatchType.Rare => level < UpgradeRareConfig.Count ? UpgradeRareConfig[level] : null,
            UpgradeInMatchType.Epic => level < UpgradeEpicConfig.Count ? UpgradeEpicConfig[level] : null,
            UpgradeInMatchType.Legendary => level < UpgradeLegendaryConfig.Count ? UpgradeLegendaryConfig[level] : null,
            UpgradeInMatchType.Percent => level < UpgradePercentConfig.Count ? UpgradePercentConfig[level] : null,
            _ => null
        };
    }
}

[System.Serializable]
public class UpgradeInMatchData
{
    [field: SerializeField, TableColumnWidth(50, false)]
    public int Level { get; private set; }
    [field: SerializeField]
    public GameResource Resource { get; private set; }
}
