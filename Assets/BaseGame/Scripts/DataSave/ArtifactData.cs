using MemoryPack;
using System.Collections;
using System.Collections.Generic;
using TW.Reactive.CustomComponent;
using UnityEngine;

[System.Serializable]
[MemoryPackable]
public partial class ArtifactData
{
    public static ArtifactData Instance => InGameDataManager.Instance.InGameData.ArtifactData;
    [field: SerializeField] public List<ReactiveValue<ArtifactInfo>> ArtifactInfos { get; set; }
}

public partial class InGameData
{
    [field: SerializeField] public ArtifactData ArtifactData { get; set; } = new();
}

[System.Serializable]
[MemoryPackable]
public partial class ArtifactInfo
{
    [field: SerializeField] public ReactiveValue<int> Id { get; set; }
    [field: SerializeField] public ReactiveValue<int> Level { get; set; }
    [field: SerializeField] public ReactiveValue<int> PiecesAmount { get; set; }
}