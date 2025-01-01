using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TW.Reactive.CustomComponent;
using TW.Utility.DesignPattern;
using UnityEngine;

public class GrowthLevelManager : Singleton<GrowthLevelManager>
{
    public ReactiveValue<int> playerLevel = new(0);
    public ReactiveValue<float> playerExp = new(0);
    public ReactiveValue<bool> isPremium = new();
    public List<GrowthDataConfig> growthDataConfigs = new();
    public GrowthDataConfig currentGrowDataConfigs = new();
    private void Start()
    {
        LoadData();
    }

    void LoadData() {
        growthDataConfigs = GrowGlobalConfig.Instance.growGlobalConfigs;
        playerLevel = InGameDataManager.Instance.InGameData.playerResourceDataSave.level;
        playerExp = InGameDataManager.Instance.InGameData.playerResourceDataSave.exp;
        isPremium = InGameDataManager.Instance.InGameData.playerResourceDataSave.premium;
        currentGrowDataConfigs = GetGrowGlobalConfig(playerLevel.Value + 1);
    }

    public GrowthDataConfig GetGrowGlobalConfig(int level) {
        foreach (GrowthDataConfig config in growthDataConfigs) {
            if (config.level == level) return config;
        }
        return null;
    }

    public void LevelUp() {
        ClaimReward();
        playerLevel.Value += 1;
        currentGrowDataConfigs = GetGrowGlobalConfig(playerLevel + 1);
        InGameDataManager.Instance.SaveData();
    }

    [Button]
    public void AddExp(float exp) {
        if (currentGrowDataConfigs != null)
        {
            playerExp.Value += exp;
            if (playerExp >= currentGrowDataConfigs.expRequire)
                LevelUp();
        } 
    }

    void ClaimReward() {
        //if (isPremium)
        //    InGameDataManager.Instance.InGameData.playerResourceDataSave.AddResourceValue(currentGrowDataConfigs.rType, currentGrowDataConfigs.premiumRewardAmount);
        //else
        //    InGameDataManager.Instance.InGameData.playerResourceDataSave.AddResourceValue(currentGrowDataConfigs.rType, currentGrowDataConfigs.commonRewardAmount);
    }
}
