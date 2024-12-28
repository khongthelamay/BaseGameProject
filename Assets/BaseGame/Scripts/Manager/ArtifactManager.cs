using R3;
using Sirenix.OdinInspector;
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
        if (artifactDataConfig == null || artifactDataConfig.id == -1)
            return;
        currentArtifactTemp = GetArtifactInfo(currentArtifactOnChoose.Value.id);
        ViewOptions options = new ViewOptions(nameof(ModalArtifactInfor));
        ModalContainer.Find(ContainerKey.Modals).Push(options);
    }

    ReactiveValue<ArtifactInfor> GetArtifactInfo(int id) {

        for (int i = 0; i < ArtifactInfos.Count; i++)
        {
            if (ArtifactInfos[i].Value.Id == id)
                return ArtifactInfos[i];
        }
        ArtifactInfor artifactInfor = new();
        artifactInfor.Id.Value = id;
        artifactInfor.Level.Value = 0;
        artifactInfor.PiecesAmount.Value = 0;

        ReactiveValue<ArtifactInfor> newArtifactInfor = new ReactiveValue<ArtifactInfor>(artifactInfor);

        ArtifactInfos.Add(newArtifactInfor);

        return newArtifactInfor;

    }

    public void UpgradeLevelArtifact() {
        currentArtifactTemp = GetArtifactInfo(currentArtifactOnChoose.Value.id);

        currentArtifactTemp.Value.PiecesAmount.Value -= currentArtifactOnChoose.Value.piecesRequire[currentArtifactTemp.Value.Level.Value];
        currentArtifactTemp.Value.Level.Value++;

        //consume coin
        //consume pieces
        //save data
        InGameDataManager.Instance.SaveData();
    }

    [Button]
    public void AddPieceArtifact(int id, int amount) {
        currentArtifactTemp = GetArtifactInfo(id);
        if (currentArtifactTemp.Value.Level.Value == 0)
            currentArtifactTemp.Value.Level.Value = 1;
        currentArtifactTemp.Value.PiecesAmount.Value += amount;
        InGameDataManager.Instance.SaveData();
    }


    public ArtifactDataConfig GetArtifactDataConfig(int id) {
        return ArtifactGlobalConfig.Instance.GetArtifactDataConfig(id);
    }

    public void IsCanUpgradeArtifact() { }

    public void ChangeCurrentArtifactInfor(ArtifactDataConfig artifactDataConfig) {
        currentArtifactOnChoose.Value = null;
        currentArtifactOnChoose.Value = artifactDataConfig;
    }
}
