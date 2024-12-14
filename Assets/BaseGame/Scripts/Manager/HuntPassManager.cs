using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TW.Reactive.CustomComponent;
using TW.Utility.DesignPattern;
using UnityEngine;

public class HuntPassManager : Singleton<HuntPassManager>
{
    public ReactiveValue<int> huntLevel;
    public ReactiveValue<int> huntPoint;
    public ReactiveList<HuntPassData> huntPassDataSave = new();
    public List<HuntPass> huntPasses = new();
    public ReactiveValue<bool> isPremium = new();
    public ReactiveValue<string> timeHuntPassOut;
    public HuntPass currentPassConfig;
    private void Start()
    {
        LoadData();
    }

    public void LoadData() {
        huntLevel = InGameDataManager.Instance.InGameData.huntPassDataSave.huntPassLevel;
        huntPoint = InGameDataManager.Instance.InGameData.huntPassDataSave.huntPoint;
        huntPasses = HuntPassGlobalConfig.Instance.huntPasses;
        huntPassDataSave = InGameDataManager.Instance.InGameData.huntPassDataSave.huntPassDatasSave;
      
        isPremium = InGameDataManager.Instance.InGameData.playerResourceDataSave.premium;
        timeHuntPassOut = InGameDataManager.Instance.InGameData.huntPassDataSave.timeOutHuntPass;

        huntLevel.ReactiveProperty.Subscribe(LevelUp).AddTo(this);

        CheckHuntTimeOut();
    }

    void CheckHuntTimeOut()
    {
        if (!string.IsNullOrEmpty(timeHuntPassOut.Value))
        {
            DateTime timeOut = DateTime.Parse(timeHuntPassOut.Value, TimeUtil.GetCultureInfo());
            TimeSpan timeRemaining = timeOut.Subtract(DateTime.Now);
            if (timeRemaining.TotalSeconds <= 0)
                SetHuntLevel(0);
        }
        else
            SaveTimeOutHuntPass();
    }

    void SaveTimeOutHuntPass() {
        timeHuntPassOut.Value = DateTime.Now.AddDays(30).ToString(TimeUtil.GetCultureInfo());
        InGameDataManager.Instance.SaveData();
    }

    [Button]
    void AddHuntPoint(int amount) {
        huntPoint.Value += amount;
        if (currentPassConfig != null && currentPassConfig.level > 0)
            if (huntPoint >= currentPassConfig.expRequire)
                SetHuntLevel(huntLevel.Value + 1);
    }

    HuntPass GetHuntPassConfig(int level) {
        foreach (HuntPass hunt in huntPasses)
        {
            if (hunt.level == level)
                return hunt;
        }
        return null;
    }

    HuntPass GetNextHuntConfig(int level)
    {
        foreach (HuntPass hunt in huntPasses)
        {
            if (hunt.level > level)
                return hunt;
        }
        return null;
    }

    void SetHuntLevel(int level) {
        huntLevel.Value = level;
        InGameDataManager.Instance.SaveData();
    }

    public void LevelUp(int level) {

        currentPassConfig = GetNextHuntConfig(huntLevel.Value);

        for (int i = 0; i < huntPassDataSave.ObservableList.Count; i++)
        {
            if (huntPassDataSave.ObservableList[i].level == level)
                return;
        }
        HuntPassData hunterPass = new();
        hunterPass.level = level;
        hunterPass.isClaimedCommond = false;
        huntPassDataSave.ObservableList.Add(hunterPass);
        InGameDataManager.Instance.SaveData();
    }

    public void ClaimHuntCommondPass(int level) {
        foreach (HuntPassData data in huntPassDataSave.ObservableList)
        {
            if (data.level == level)
            {
                data.isClaimedCommond = true;
                HuntPass huntPass = GetHuntPassConfig(level);
                //if (huntPass != null)
                //    InGameDataManager.Instance.InGameData.playerResourceDataSave.AddResourceValue(huntPass.rType, huntPass.commonRewardAmount);
                return;
            }
        }
    }

    public void ClaimHuntPremieumPass(int level)
    {
        foreach (HuntPassData data in huntPassDataSave.ObservableList)
        {
            if (data.level == level)
            {
                data.isClaimedPremium = true;
                HuntPass huntPass = GetHuntPassConfig(level);
                //if (huntPass != null)
                //    InGameDataManager.Instance.InGameData.playerResourceDataSave.AddResourceValue(huntPass.rType, huntPass.premiumRewardAmount);
                return;
            }
        }
    }

    public bool IsClaimedComond(int level)
    {
        foreach (HuntPassData data in huntPassDataSave.ObservableList)
        {
            if (data.level == level && data.isClaimedCommond)
                return true;
        }
        return false;
    }

    public bool IsClaimedPremium(int level)
    {
        foreach (HuntPassData data in huntPassDataSave.ObservableList)
        {
            if (data.level == level && data.isClaimedPremium)
                return true;
        }
        return false;
    }

    public bool IsLock(int level)
    {
        foreach (HuntPassData data in huntPassDataSave.ObservableList)
        {
            if (data.level == level) return false;
        }
        return true;
    }
}
