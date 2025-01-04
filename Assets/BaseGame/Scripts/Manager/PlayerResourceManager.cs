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
    [field: SerializeField] public List<Resource> resources { get; set; } = new();
    [field: SerializeField] public List<ResourceSave> resourceSave { get; set; } = new();
    public ReactiveValue<DateTime> timeEneryDone { get; set; } = new(default(DateTime));
    public ReactiveValue<DateTime> timeEnegyAddOne { get; set; } = new(default(DateTime));

    public bool coolDown = false;
    [ReadOnly] public float timeCoolDown = -1;

    private void Start()
    {
        LoadData();
    }

    private void LoadData()
    {
        timeCoolDown = -1;

        resourceSave = InGameDataManager.Instance.InGameData.playerResourceDataSave.resourceSaves;

        for (int i = 0; i < resourceSave.Count; i++)
        {
            Resource resource = new();
            resource.type = resourceSave[i].type;
            resource.value.Value = new(resourceSave[i].coefficient, resourceSave[i].exp);
            resources.Add(resource);
        }   

        level = InGameDataManager.Instance.InGameData.playerResourceDataSave.level;
        exp = InGameDataManager.Instance.InGameData.playerResourceDataSave.exp;

        timeEneryDone = InGameDataManager.Instance.InGameData.playerResourceDataSave.timeEnergyDone;
        timeEnegyAddOne = InGameDataManager.Instance.InGameData.playerResourceDataSave.timeEnegyAddOne;
        CheckTimeEnergy();
    }

    private void FixedUpdate()
    {
        if (coolDown)
        {
            if (timeCoolDown >= 0)
            {
                timeCoolDown = (float)timeEnegyAddOne.Value.Subtract(DateTime.Now).TotalSeconds;
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
        if (timeEneryDone == null)
        {
            if (resource.value.Value == 0) SetResource(ResourceType.Energy, 30);
            return;
        }
        
        if (timeEneryDone.Value.Subtract(DateTime.Now).TotalSeconds < 0)
        {
            if (resource.value.Value < 30) SetResource(ResourceType.Energy, 30);
        }
        CheckTimeToGetOneEnergy();
    }

    void CheckTimeToGetOneEnergy() {
        if (timeEnegyAddOne == null)
            return;
        if (timeEnegyAddOne.Value.Subtract(DateTime.Now).TotalSeconds > 0 && !coolDown)
        {
            timeCoolDown = (float)timeEnegyAddOne.Value.Subtract(DateTime.Now).TotalSeconds;
            coolDown = true;
        }
        else 
        {
            Resource resource = GetResource(ResourceType.Energy);
            if (resource.value.Value < 30 )
            {
                if (!coolDown)
                {
                    timeEnegyAddOne.Value = timeEnegyAddOne.Value.AddMinutes(.5f);
                    timeCoolDown = (float)timeEnegyAddOne.Value.Subtract(DateTime.Now).TotalSeconds;
                    coolDown = true;
                }
            }
            else
            {
                timeEneryDone.Value = default(DateTime);
                ScreensDefaultContext.Events.TurnOffTimeRemaining?.Invoke();
            }
        }
    }

    [Button]
    public void ChangeResource(ResourceType rType, BigNumber amount) {
        for (int i = 0; i < resources.Count; i++)
        {
            if (resources[i].type == rType)
            {
                resources[i].ChangeValue(amount);
                resourceSave[i].coefficient = resources[i].value.Value.coefficient;
                resourceSave[i].exp = resources[i].value.Value.exponent;
                InGameDataManager.Instance.SaveData();
                return;
            }
        }

        Resource resource = new();
        resource.type = rType;
        resource.value.Value = amount;

        ResourceSave newResourceSave = new();
        newResourceSave.type = rType;
        newResourceSave.coefficient = amount.coefficient;
        newResourceSave.exp = amount.exponent;

        resources.Add(resource);
        resourceSave.Add(newResourceSave);

        InGameDataManager.Instance.SaveData();
    }

    [Button]
    public void ConsumeEnergy(BigNumber amount) {
        Resource resource = GetResource(ResourceType.Energy);

        resource.Consume(amount);
        if (timeEneryDone.Value == default(DateTime))
            timeEneryDone.Value = DateTime.Now;
        timeEneryDone.Value = timeEneryDone.Value.AddMinutes(.5f * amount.ToFloat());
        if (!coolDown)
            timeEnegyAddOne.Value = DateTime.Now.AddMinutes(.5f);
        CheckTimeToGetOneEnergy();
        InGameDataManager.Instance.SaveData();
    }

    public void SetResource(ResourceType rType, BigNumber amount)
    {
        Resource resource = GetResource(rType);
        resource.value.Value = amount;
        InGameDataManager.Instance.SaveData();
    }

    public Resource GetResource(ResourceType rType) {
        for (int i = 0; i < resources.Count; i++)
        {
            if (resources[i].type == rType)
            {
                return resources[i];
            }
        }

        ChangeResource(rType, 0);
        return resources[^1];
    }
}
