using MemoryPack;
using System;
using System.Collections;
using System.Collections.Generic;
using TW.Reactive.CustomComponent;
using UnityEngine;

[System.Serializable]
[MemoryPackable]
public partial class ArtifactDataSave
{
    public static ArtifactDataSave Instance => InGameDataManager.Instance.InGameData.ArtifactData;
    [field: SerializeField] public List<ReactiveValue<ArtifactInfor>> ArtifactInfos { get; set; } = new();

    public ArtifactInfor GetArtifactInfor(ArtifactType artifactType)
    {
        for (int i = 0; i < ArtifactInfos.Count; i++)
        {
            if (ArtifactInfos[i].Value.Id.Value == (int)artifactType)
                return ArtifactInfos[i].Value;
        }
        return null;
    }

    public bool IsHaveThatArtiFact(ArtifactType artifactType) {
        for (int i = 0; i < ArtifactInfos.Count; i++)
        {
            if (ArtifactInfos[i].Value.Id.Value == (int)artifactType && ArtifactInfos[i].Value.Level.Value > 0)
                return true;
        }
        return false;
    }
}

public partial class InGameData
{
    [MemoryPackOrder(102)]
    [field: SerializeField] public ArtifactDataSave ArtifactData { get; set; } = new();
}

[System.Serializable]
[MemoryPackable]
public partial class ArtifactInfor
{
    [field: SerializeField] public ReactiveValue<int> Id { get; set; } = new(0);
    [field: SerializeField] public ReactiveValue<int> Level { get; set; } = new(0);
    [field: SerializeField] public ReactiveValue<int> PiecesAmount { get; set; } = new(0);
}