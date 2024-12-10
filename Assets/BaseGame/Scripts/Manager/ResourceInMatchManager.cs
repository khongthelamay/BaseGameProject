using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TW.Reactive.CustomComponent;
using TW.Utility.CustomType;
using TW.Utility.DesignPattern;
using UnityEngine;

public class ResourceInMatchManager : Singleton<ResourceInMatchManager>
{
    public List<ReactiveValue<Resource>> resourceInMatch = new();

    private void OnEnable()
    {
        PlayGame();
    }
    public void PlayGame() {
        ReactiveValue<Resource> reactiveValue = GetResource(ResourceType.CoinInMatch);
        reactiveValue.Value.Add(150);
    }

    public ReactiveValue<Resource> GetResource(ResourceType rType) {
        for (int i = 0; i < resourceInMatch.Count; i++)
        {
            if (resourceInMatch[i].Value.type == rType)
            {
                return resourceInMatch[i];
            }
        }
        return null;
    }

    [Button]
    public void AddResource(ResourceType rType, BigNumber amount)
    {
        for (int i = 0; i < resourceInMatch.Count; i++)
        {
            if (resourceInMatch[i].Value.type == rType)
            {
                resourceInMatch[i].Value.Add(amount);
                break;
            }
        }
    }

    [Button]
    public void ConsumeResource(ResourceType rType, BigNumber amount) {
        for (int i = 0; i < resourceInMatch.Count; i++)
        {
            if (resourceInMatch[i].Value.type == rType)
            {
                resourceInMatch[i].Value.Consume(amount);
                break;
            }
        }
    }

    public bool IsEnough(ResourceType rType, BigNumber amount) {
        for (int i = 0; i < resourceInMatch.Count; i++)
        {
            if (resourceInMatch[i].Value.type == rType)
            {
                return resourceInMatch[i].Value.IsEnough(amount);
            }
        }
        return false;
    }

    public UpgradeInMatchData GetUpgradeInMatchConfig(int level, UpgradeInMatchType upgradeInMatchType) {
        return UpgradeInMatchGlobalConfig.Instance.GetUpgradeInMatchData(level, upgradeInMatchType);
    }
}
