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
    [field: SerializeField] public ReactiveValue<string> timeEnegyAddOne { get; set; } = new();

    public bool coolDown = false;
    [ReadOnly] public float timeCoolDown = -1;

    private void Start()
    {
        LoadData();
    }

    private void LoadData()
    {
        timeCoolDown = -1;
        level = PlayerResourceDataSave.Instance.level;
        exp = PlayerResourceDataSave.Instance.exp;
        resources = PlayerResourceDataSave.Instance.resources;
        timeEneryDone = PlayerResourceDataSave.Instance.timeEnergyDone;
        timeEnegyAddOne = PlayerResourceDataSave.Instance.timeEnegyAddOne;
        CheckTimeEnergy();
    }

    private void FixedUpdate()
    {
        if (coolDown)
        {
            if (timeCoolDown >= 0)
            {
                timeCoolDown -= Time.deltaTime;

            }
            else {
                coolDown = false;
                ChangeResource(ResourceType.Energy, 1);
                CheckTimeToGetOneEnergy();
            }

            ScreensDefaultContext.Events.ChangeTimeRemaining?.Invoke(timeCoolDown);
        }
    }

    void CheckTimeEnergy() {
        Resource resource = GetResource(ResourceType.Energy);
        if (string.IsNullOrEmpty(timeEneryDone.Value))
        {
            if (resource.value.Value == 0) SetResource(ResourceType.Energy, 30);
            return;
        }


        DateTime timeConvert = DateTime.Parse(timeEneryDone.Value, TimeUtil.GetCultureInfo());
        if (timeConvert.Subtract(DateTime.Now).TotalSeconds < 0)
        {
            if (resource.value.Value < 30) SetResource(ResourceType.Energy, 30);
        }
        CheckTimeToGetOneEnergy();
    }

    void CheckTimeToGetOneEnergy() {
        if (string.IsNullOrEmpty(timeEnegyAddOne.Value))
            return;
        DateTime timeConvert = DateTime.Parse(timeEnegyAddOne.Value, TimeUtil.GetCultureInfo());
        if (timeConvert.Subtract(DateTime.Now).TotalSeconds > 0 && !coolDown)
        {
            timeCoolDown = (float)timeConvert.Subtract(DateTime.Now).TotalSeconds;
            coolDown = true;
        }
        else 
        {
            Resource resource = GetResource(ResourceType.Energy);
            if (resource.value.Value < 30 )
            {
                if (!coolDown)
                {
                    timeConvert = DateTime.Parse(timeEnegyAddOne.Value, TimeUtil.GetCultureInfo());
                    timeEnegyAddOne.Value = timeConvert.AddMinutes(.5f).ToString(TimeUtil.GetCultureInfo());
                    timeConvert = DateTime.Parse(timeEnegyAddOne.Value, TimeUtil.GetCultureInfo());
                    timeCoolDown = (float)timeConvert.Subtract(DateTime.Now).TotalSeconds;
                    coolDown = true;
                }
            }
            else
            {
                timeEneryDone.Value = "";
                ScreensDefaultContext.Events.TurnOffTimeRemaining?.Invoke();
            }
        }
    }

    [Button]
    public void ChangeResource(ResourceType rType, BigNumber amount) {
        Debug.Log(coolDown);
        Debug.Log($"Change resource {rType} amount: {amount}");
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
        DateTime timeConvert = string.IsNullOrEmpty(timeEneryDone.Value) ? DateTime.Now : DateTime.Parse(timeEneryDone.Value, TimeUtil.GetCultureInfo());
        timeEneryDone.Value = timeConvert.AddMinutes(.5f * amount.ToFloat()).ToString(TimeUtil.GetCultureInfo());
        if (!coolDown)
            timeEnegyAddOne.Value = timeConvert.AddMinutes(.5f).ToString(TimeUtil.GetCultureInfo());
        CheckTimeToGetOneEnergy();
        InGameDataManager.Instance.SaveData();
    }

    public void SetResource(ResourceType rType, BigNumber amount)
    {
        Debug.Log($"set resource: {rType} amout: {amount}");
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
