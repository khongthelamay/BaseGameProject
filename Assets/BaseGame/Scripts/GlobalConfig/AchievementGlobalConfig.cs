using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using TW.Utility.CustomType;
using UnityEngine;

[CreateAssetMenu(fileName = "AchievementGlobalConfig", menuName = "GlobalConfigs/AchievementGlobalConfig")]
[GlobalConfig("Assets/Resources/GlobalConfig/")]
public class AchievementGlobalConfig : GlobalConfig<AchievementGlobalConfig>
{
    public List<AchievementDataConfig> achievementDataConfigs = new();

}

[System.Serializable]
public class AchievementDataConfig {
    public int id;
    public string strDes;
    public BigNumber requireBase;
    public bool isCanScale;
    public int scaleBase;
    public float reward;
}
