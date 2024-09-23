using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TW.Reactive.CustomComponent;
using TW.Utility.DesignPattern;
using UnityEngine;

public class ArtifactManager : Singleton<ArtifactManager>
{
    [field: SerializeField] public List<ReactiveValue<ArtifactInfo>> ArtifactInfos { get; set; }
    ReactiveValue<ArtifactInfo> artifactInforTemp;

    private void Start()
    {
        LoadData();
    }

    void LoadData() {
        ArtifactInfos = InGameDataManager.Instance.InGameData.ArtifactData.ArtifactInfos;
    }

    ReactiveValue<ArtifactInfo> GetArtifactInfo(ArtifactType artifactType) {

        for (int i = 0; i < ArtifactInfos.Count; i++)
        {
            if (ArtifactInfos[i].Value.Id == (int)artifactType)
                return ArtifactInfos[i];
        }

        ReactiveValue<ArtifactInfo> newArtifactInfor = new ReactiveValue<ArtifactInfo>();
        newArtifactInfor.Value.Id = new((int)artifactType);
        newArtifactInfor.Value.Level = new(0);
        newArtifactInfor.Value.PiecesAmount = new(0);

        ArtifactInfos.Add(newArtifactInfor);

        return newArtifactInfor;

    }

    public void UpgradeLevelArtifact(ArtifactType artifactType) {
        artifactInforTemp = GetArtifactInfo(artifactType);
        artifactInforTemp.Value.Level.Value++;
        //consume coin
        //consume pieces
        //save data
        InGameDataManager.Instance.SaveData();
    }

    public void AddPieceArtifact(ArtifactType artifactType, int amount) {
        artifactInforTemp = GetArtifactInfo(artifactType);
        artifactInforTemp.Value.PiecesAmount.Value += amount;
        InGameDataManager.Instance.SaveData();
    }


    public ArtifactDataConfig GetArtifactDataConfig(ArtifactType artifactType) {
        return ArtifactGlobalConfig.Instance.GetArtifactDataConfig(artifactType);
    }
}
