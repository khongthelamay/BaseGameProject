using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using TW.Reactive.CustomComponent;
using TW.Utility.DesignPattern;
using UnityEngine;

public class HunterPassManager : Singleton<HunterPassManager>
{
    public ReactiveValue<int> level;
    public ReactiveValue<int> huntPoint;
    public ReactiveList<HunterPassData> hunterPassDataSave = new();
    public List<HunterPass> hunterPasses = new();
    public bool isPremium;

    private void Start()
    {
        LoadData();
    }

    public void LoadData() {
        level = InGameDataManager.Instance.InGameData.playerResourceDataSave.level;
        hunterPasses = HunterPassGlobalConfig.Instance.hunterPasses;
        hunterPassDataSave = InGameDataManager.Instance.InGameData.hunterPassDataSave.hunterPassDataSave;
        level.ReactiveProperty.Subscribe(LevelUp).AddTo(this);
    }

    public void LevelUp(int level) {
        for (int i = 0; i < hunterPassDataSave.ObservableList.Count; i++)
        {
            if (hunterPassDataSave.ObservableList[i].level == level)
                return;
        }
        HunterPassData hunterPass = new();
        hunterPass.level = level;
        hunterPass.isClaimedCommond = false;
        hunterPassDataSave.ObservableList.Add(hunterPass);
        InGameDataManager.Instance.SaveData();
    }

    public void ClaimHuntCommondPass(int level) {
        foreach (HunterPassData data in hunterPassDataSave.ObservableList)
        {
            if (data.level == level)
            {
                data.isClaimedCommond = true;
                return;
            }
        }
    }

    public void ClaimHuntPremieumPass(int level)
    {
        foreach (HunterPassData data in hunterPassDataSave.ObservableList)
        {
            if (data.level == level)
            {
                data.isClaimedCommond = true;
                return;
            }
        }
    }

    public bool IsClaimedComond(int level)
    {
        foreach (HunterPassData data in hunterPassDataSave.ObservableList)
        {
            if (data.level == level && data.isClaimedCommond)
                return true;
        }
        return false;
    }

    public bool IsClaimedPremium(int level)
    {
        foreach (HunterPassData data in hunterPassDataSave.ObservableList)
        {
            if (data.level == level && data.isClaimedPremium)
                return true;
        }
        return false;
    }

    public bool IsLock(int level)
    {
        foreach (HunterPassData data in hunterPassDataSave.ObservableList)
        {
            if (data.level == level) return false;
        }
        return true;
    }
}
