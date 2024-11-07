using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using System.Collections;
using System.Collections.Generic;
using TW.Reactive.CustomComponent;
using TW.Utility.DesignPattern;
using UnityEngine;

public class HunterPassManager : Singleton<HunterPassManager>
{
    public ReactiveValue<int> level;
    public ReactiveList<HunterPassData> hunterPassDataSave = new();
    public List<HunterPass> hunterPasses = new();

    private void Start()
    {
        LoadData();
    }

    public void LoadData() {
        hunterPasses = HunterPassGlobalConfig.Instance.hunterPasses;
        hunterPassDataSave = InGameDataManager.Instance.InGameData.hunterPassDataSave.hunterPassDataSave;
    }

    public void LevelUp() { }

    public void AddHunterPass() { }

    public void RemoveHunterPass() { }
}
