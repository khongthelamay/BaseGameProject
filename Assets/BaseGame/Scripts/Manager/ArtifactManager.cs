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

    public ReactiveValue<ArtifactDataConfig> currentArtifactOnChoose { get; set; } = new();

    private void Start()
    {
        LoadData();
    }

    void LoadData() {
        ArtifactInfos = InGameDataManager.Instance.InGameData.ArtifactData.ArtifactInfos;
        if (ArtifactInfos.Count == 0)
        {
            List<ArtifactDataConfig> artifactGlobalConfigs = ArtifactGlobalConfig.Instance.artifactDataConfigs;
            for (int i = 0; i < artifactGlobalConfigs.Count; i++)
            {
                GetArtifactInfo(artifactGlobalConfigs[i].id);
            }
        }
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
        
        ArtifactInfor info = new();
        info.Id.Value = id;
        info.Level.Value = 0;
        info.PiecesAmount.Value = 0;

        ReactiveValue<ArtifactInfor> newArtifactInfo = new ReactiveValue<ArtifactInfor>(info);

        ArtifactInfos.Add(newArtifactInfo);

        return newArtifactInfo;

    }

    public void UpgradeLevelArtifact() {
        PlayerResourceManager.Instance.ChangeResource(ResourceType.Coin,-GetPriceUpgrade(currentArtifactTemp.Value.Level.Value));

        currentArtifactTemp = GetArtifactInfo(currentArtifactOnChoose.Value.id);

        currentArtifactTemp.Value.PiecesAmount.Value -= GetPiecesRequire(currentArtifactTemp.Value.Level.Value);
        currentArtifactTemp.Value.Level.Value++;

        //consume coin
        //consume pieces
        //save data
        InGameDataManager.Instance.SaveData();
    }

    public int GetPiecesRequire(int level)
    {
        return ArtifactGlobalConfig.Instance.piecesRequire[level];
    }
    
    public float GetPriceUpgrade(int level)
    {
        return ArtifactGlobalConfig.Instance.priceUpgrade[level];
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

    public bool IsCanUpgradeArtifact(ArtifactDataConfig artifactDataConfig, ArtifactInfor artifactInfo)
    {
        return false;
    }

    public void ChangeCurrentArtifactInfo(ArtifactDataConfig artifactDataConfig) {
        Debug.Log("Change artifact info");
        currentArtifactOnChoose.Value = null;
        currentArtifactOnChoose.Value = artifactDataConfig;
    }
}
