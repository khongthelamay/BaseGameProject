using R3;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TW.Reactive.CustomComponent;
using TW.UGUI.Core.Modals;
using TW.UGUI.Core.Views;
using TW.Utility.DesignPattern;
using UnityEngine;

public class ArtifactManager : Singleton<ArtifactManager>
{
    [field: SerializeField] public List<ReactiveValue<ArtifactInfor>> ArtifactInfos { get; set; } = new();

    public ReactiveValue<ArtifactInfor> currentArtifactTemp = new();

    public ReactiveValue<ArtifactDataConfig> currentArtifactOnChoose = new();

    private void Start()
    {
        LoadData();
    }

    void LoadData() {
        ArtifactInfos = InGameDataManager.Instance.InGameData.ArtifactData.ArtifactInfos;
        currentArtifactOnChoose.ReactiveProperty.Subscribe(ChangeCurrentArtifactChoose).AddTo(this);
    }

    void ChangeCurrentArtifactChoose(ArtifactDataConfig artifactDataConfig)
    {
        if (artifactDataConfig == null || artifactDataConfig.artifactType == ArtifactType.None)
            return;
        currentArtifactTemp = GetArtifactInfo(currentArtifactOnChoose.Value.artifactType);
        ViewOptions options = new ViewOptions(nameof(ModalArtifactInfor));
        ModalContainer.Find(ContainerKey.Modals).Push(options);
    }

    ReactiveValue<ArtifactInfor> GetArtifactInfo(ArtifactType artifactType) {

        for (int i = 0; i < ArtifactInfos.Count; i++)
        {
            if (ArtifactInfos[i].Value.Id == (int)artifactType)
                return ArtifactInfos[i];
        }

        ReactiveValue<ArtifactInfor> newArtifactInfor = new ReactiveValue<ArtifactInfor>();
        newArtifactInfor.Value.Id = new((int)artifactType);
        newArtifactInfor.Value.Level = new(0);
        newArtifactInfor.Value.PiecesAmount = new(0);

        ArtifactInfos.Add(newArtifactInfor);

        return newArtifactInfor;

    }

    public void UpgradeLevelArtifact() {
        currentArtifactTemp = GetArtifactInfo(currentArtifactOnChoose.Value.artifactType);
        currentArtifactTemp.Value.Level.Value++;
        //consume coin
        //consume pieces
        //save data
        InGameDataManager.Instance.SaveData();
    }

    public void AddPieceArtifact(ArtifactType artifactType, int amount) {
        currentArtifactTemp = GetArtifactInfo(artifactType);
        currentArtifactTemp.Value.PiecesAmount.Value += amount;
        InGameDataManager.Instance.SaveData();
    }


    public ArtifactDataConfig GetArtifactDataConfig(ArtifactType artifactType) {
        return ArtifactGlobalConfig.Instance.GetArtifactDataConfig(artifactType);
    }

    public void IsCanUpgradeArtifact() { }
}
