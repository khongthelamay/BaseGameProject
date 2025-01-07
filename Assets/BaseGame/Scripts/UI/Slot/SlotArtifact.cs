using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Sirenix.OdinInspector.Editor;

public class SlotArtifact : SlotBase <ArtifactDataConfig>
{
    [Header(" ======ArtifactSlot====== ")]
    [SerializeField] Image imgBG;
    [SerializeField] TextMeshProUGUI txtName;
    [SerializeField] TextMeshProUGUI txtLevel;
    [SerializeField] ProgressBar progressBar;
    ArtifactInfor artifactInfo;
    public override void InitData(ArtifactDataConfig data)
    {
        base.InitData(data);
        artifactInfo = InGameDataManager.Instance.InGameData.ArtifactData.GetArtifactInfor(data.id);

        int level = artifactInfo.Level.Value;
        txtLevel.text = $"Lv. {level}";

        if (artifactInfo.Level.Value < ArtifactGlobalConfig.Instance.piecesRequire.Count)
        {
            progressBar.ChangeTextProgress($"{artifactInfo.PiecesAmount.Value}/{ArtifactManager.Instance.GetPiecesRequire(level)}");
        }
        else {
            progressBar.ChangeTextProgress("MAX");
        }
    }

    public override void AnimOpen()
    {
        if (mySequence != null) mySequence.Kill();
        mySequence = DOTween.Sequence();
        mySequence = UIAnimation.AnimSlotPopUp(trsContent);
        mySequence.OnComplete(() =>
        {
            progressBar.ChangeProgress((float)artifactInfo.PiecesAmount.Value /
                                       (float)ArtifactManager.Instance.GetPiecesRequire(artifactInfo.Level.Value));
        });
    }

    public override void ReloadData()
    {
        InitData(slotData);
    }
    public override void CleanAnimation()
    {
        base.CleanAnimation();
        progressBar.ClearAnimation();
    }
}
