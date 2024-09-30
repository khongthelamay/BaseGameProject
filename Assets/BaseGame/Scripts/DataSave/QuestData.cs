using MemoryPack;
using System.Collections;
using System.Collections.Generic;
using TW.Reactive.CustomComponent;
using UnityEngine;

[System.Serializable]
[MemoryPackable]
public partial class QuestData
{
    public static QuestData Instance => InGameDataManager.Instance.InGameData.QuestData;
    [field: SerializeField] public List<ReactiveValue<QuestSave>> questSaves { get; set; } = new();
}
public partial class InGameData
{
    [field: SerializeField] public QuestData QuestData { get; set; } = new();
}

[System.Serializable]
[MemoryPackable]
public partial class QuestSave
{
    [field: SerializeField] public ReactiveValue<int> id { get; set; } = new(0);
    [field: SerializeField] public ReactiveValue<int> progress { get; set; } = new(0);
    [field: SerializeField] public ReactiveValue<bool> claimed { get; set; } = new(false);
}
