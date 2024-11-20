using MemoryPack;
using TW.Reactive.CustomComponent;
using TW.Utility.CustomType;
using UnityEngine;

[System.Serializable]
[MemoryPackable]
public partial class PlayerResourceDataSave
{
    public static PlayerResourceDataSave Instance => InGameDataManager.Instance.InGameData.playerResourceDataSave;
    [field: SerializeField] public ReactiveValue<int> level;
    [field: SerializeField] public ReactiveValue<float> exp;
    [field: SerializeField] public ReactiveValue<bool> premium;
    [field: SerializeField] public ReactiveList<Resource> resources { get; set; } = new();

    public ReactiveValue<BigNumber> GetResourceValue(ResourceType resourceType) 
    {
        for (int i = 0; i < resources.ObservableList.Count; i++)
        {
            if (resources.ObservableList[i].type == resourceType)
                return resources.ObservableList[i].value;
        }
        Resource newResource = new();
        newResource.type = resourceType;
        newResource.value = new(0);
        AddResource(newResource);
        return null;
    }

    public void AddResource(Resource resource) {
        resources.ObservableList.Add(resource);
        InGameDataManager.Instance.SaveData();
    }

    public void AddResourceValue(ResourceType type, BigNumber value)
    {
        for (int i = 0; i < resources.ObservableList.Count; i++)
        {
            if (resources.ObservableList[i].type == type)
            {
                resources.ObservableList[i].Add(value);
                InGameDataManager.Instance.SaveData();
                return;
            }
        }

        Resource resource = new Resource();
        resource.type = type;
        resource.value.Value = value;
        AddResource(resource);
        InGameDataManager.Instance.SaveData();
    }

    public void ConsumeResourceValue(ResourceType type, BigNumber value)
    {
        for (int i = 0; i < resources.ObservableList.Count; i++)
        {
            if (resources.ObservableList[i].type == type)
                resources.ObservableList[i].Consume(value);
        }
        InGameDataManager.Instance.SaveData();
    }
}
public partial class InGameData
{
    [MemoryPackOrder(105)]
    [field: SerializeField] public PlayerResourceDataSave playerResourceDataSave { get; set; } = new();
}
