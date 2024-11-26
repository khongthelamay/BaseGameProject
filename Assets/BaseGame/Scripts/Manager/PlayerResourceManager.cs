using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TW.Reactive.CustomComponent;
using TW.Utility.CustomType;
using TW.Utility.DesignPattern;
using UnityEngine;

public class PlayerResourceManager : Singleton<PlayerResourceManager>
{
    [field: SerializeField] public ReactiveValue<int> level { get; set; } = new(0);
    [field: SerializeField] public ReactiveValue<float> exp { get; set; } = new(0);
    [field: SerializeField] public ReactiveList<Resource> resources { get; set; } = new();
    [field: SerializeField] public ReactiveValue<string> timeEneryDone { get; set; } = new();

    private void Start()
    {
        LoadData();
    }

    private void LoadData()
    {
        level = PlayerResourceDataSave.Instance.level;
        exp = PlayerResourceDataSave.Instance.exp;
        resources = PlayerResourceDataSave.Instance.resources;
        timeEneryDone = PlayerResourceDataSave.Instance.lastTimeEnergy;
        CheckTimeEnergy();
    }

    void CheckTimeEnergy() {
        Resource resource = GetResource(ResourceType.Energy);
        if (string.IsNullOrEmpty(timeEneryDone.Value))
        {
            if (resource.value.Value == 0) ChangeResource(ResourceType.Energy, 30);
            return;
        }


        DateTime timeConvert = DateTime.Parse(timeEneryDone.Value, TimeUtil.GetCultureInfo());
        if (timeConvert.Subtract(DateTime.Now).TotalSeconds < 0)
        {
            //if (resource.value.Value == 0) SetResource(ResourceType.Energy, 30);
        }
    }

    [Button]
    public void ChangeResource(ResourceType rType, BigNumber amount) {
        for (int i = 0; i < resources.ObservableList.Count; i++)
        {
            if (resources.ObservableList[i].type == rType)
            {
                resources.ObservableList[i].ChangeValue(amount);
                InGameDataManager.Instance.SaveData();
                return;
            }
        }

        Resource resource = new();
        resource.type = rType;
        resource.value.Value = amount;

        resources.ObservableList.Add(resource);
        InGameDataManager.Instance.SaveData();
    }

    [Button]
    public void ComsumeEnery(BigNumber amount) {
        Resource resource = GetResource(ResourceType.Energy);
        resource.Consume(amount);
        timeEneryDone.Value = DateTime.Now.AddMinutes(10f).ToString(TimeUtil.GetCultureInfo());
        InGameDataManager.Instance.SaveData();
    }

    public void SetResource(ResourceType rType, BigNumber amount)
    {
        Resource resource = GetResource(rType);
        resource.value.Value = amount;
        InGameDataManager.Instance.SaveData();
    }

    public Resource GetResource(ResourceType rType) {
        for (int i = 0; i < resources.ObservableList.Count; i++)
        {
            if (resources.ObservableList[i].type == rType)
            {
                return resources.ObservableList[i];
            }
        }

        ChangeResource(rType, 0);
        return resources.ObservableList[resources.ObservableList.Count - 1];
    }
}
