using System.Collections;
using System.Collections.Generic;
using TW.Reactive.CustomComponent;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ArtifactSlot : SlotBase <ArtifactDataConfig>
{
    [Header(" ======ArtifactSlot====== ")]
    [SerializeField] Image imgBG;
    [SerializeField] TextMeshProUGUI txtName;
    [SerializeField] TextMeshProUGUI txtLevel;
    [SerializeField] ProgressBar progressBar;
    public bool isHaveThatArtifact;
    ArtifactInfor artifactInfor;
    public override void InitData(ArtifactDataConfig data)
    {
        base.InitData(data);
        artifactInfor = InGameDataManager.Instance.InGameData.ArtifactData.GetArtifactInfor(data.artifactType);
        txtName.text = data.strName;
        

        isHaveThatArtifact = artifactInfor == null;
        if (isHaveThatArtifact) {
            artifactInfor = new();
            artifactInfor.Level = new(0);
            artifactInfor.PiecesAmount = new(0);
        }

        txtLevel.text = $"Lv. {artifactInfor.Level.Value}";

        if (artifactInfor.Level.Value < data.piecesRequire.Count)
        {
            progressBar.ChangeTextProgress($"{artifactInfor.PiecesAmount.Value}/{data.piecesRequire[artifactInfor.Level]}");
            progressBar.ChangeProgress((float)artifactInfor.PiecesAmount.Value / (float)data.piecesRequire[artifactInfor.Level]);
        }
        else {
            progressBar.ChangeTextProgress("MAX");
            progressBar.ChangeProgress(1f);
        }
       
    }

    public override void ReloadData()
    {
        InitData(slotData);
    }
}
