using DG.Tweening;
using Spine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using TMPro;
using TW.Utility.CustomType;
using UnityEngine;
using UnityEngine.UI;

public class SlotAchievement : SlotBase<AchievementDataConfig>
{
    [Header("==== AchievementSlot ====")]
    AchievementSave achievementSave = new();

    [SerializeField] RectMask2D myMask;
    [SerializeField] TextMeshProUGUI txtDes;
    [SerializeField] TextMeshProUGUI txtProgress;
    [SerializeField] TextMeshProUGUI txtReward;
    [SerializeField] Image imgBG;
    [SerializeField] ProgressBar progressBar;
    [SerializeField] LayoutElement myLayout;

    [SerializeField] GameObject objProgressDone;
    [SerializeField] GameObject objNotice;
    [SerializeField] GameObject objTextNotice;
    [SerializeField] GameObject objBGCanClaim;
    [SerializeField] GameObject objLight;

    [SerializeField] Transform trsProgressDone;
    [SerializeField] Transform trsIconProgressDone;
    [SerializeField] Transform trsLight;
    [SerializeField] Transform pointLightStart;
    [SerializeField] Transform pointLightEnd;

    [SerializeField] Vector3 vectorRotate;

    bool onClaim;

    float heightDefault;
    public override void Awake()
    {
        base.Awake();
        heightDefault = myLayout.preferredHeight;
    }

    public bool IsClaimed() { return objProgressDone.activeSelf; }

    public bool IsCanClaim() { return btnChoose.interactable; }

    public override void InitData(AchievementDataConfig data)
    {
        if (onClaim) return;
        base.InitData(data);
        animOnSlot.enabled = true;

        achievementSave = AchievementManager.Instance.GetAchievementDataSave(slotData.achievementType);

        txtReward.text = data.reward.value.Value.ToString();
        txtDes.text = string.Format(data.strDes, data.require);
        txtProgress.text = $"{(BigNumber)achievementSave.currentProgress.Value} / {(BigNumber)data.require}";

        progressBar.ChangeProgress((float)achievementSave.currentProgress.Value / (float)data.require);

        btnChoose.interactable = AchievementManager.Instance.IsCanClaim(achievementSave);

        objBGCanClaim.SetActive(btnChoose.interactable);
        objNotice.SetActive(btnChoose.interactable);
        objTextNotice.SetActive(btnChoose.interactable);
        objProgressDone.SetActive(false);

        if (!AchievementManager.Instance.IsCanUpdateLevelAchievement(
            (AchievementType)achievementSave.achievementType.Value, 
            achievementSave.achievementLevel.Value))
            ActionCallBackClaimed();
    }

    void ActionCallBackClaimed()
    {
        objProgressDone.SetActive(true);
        myLayout.preferredHeight = heightDefault;
        transform.SetAsLastSibling();
    }

    public override void AnimOpen()
    {
        if (mySequence != null) mySequence.Kill();

        mySequence = DOTween.Sequence();

        trsContent.localScale = Vector3.one;

        myMask.enabled = true;

        mySequence.Append(UIAnimation.AnimSlotVerticalOpen(myLayout, heightDefault, () => { myMask.enabled = false; }));
    }

    public override void AnimDone()
    {
        if (mySequence != null) mySequence.Kill();
        onClaim = true;

        btnChoose.interactable = false;

        objLight.SetActive(true);
        objNotice.SetActive(false);

        myMask.enabled = true;
        animOnSlot.enabled = false;

        mySequence = DOTween.Sequence();
        mySequence.Append(trsLight.DOLocalMove(pointLightEnd.localPosition, .5f).From(pointLightStart.localPosition).OnComplete(() =>
        {
            objProgressDone.SetActive(true);
            objTextNotice.SetActive(false);
            objLight.SetActive(false);
            objBGCanClaim.SetActive(false);
        }));

        mySequence.Append(trsProgressDone.DOScale(Vector3.one * 2f, .15f));
        mySequence.Append(trsProgressDone.DOScale(Vector3.one, .15f));
        mySequence.Append(trsIconProgressDone.DORotate(Vector3.zero, .15f).From(vectorRotate));
        mySequence.Append(UIAnimation.AnimSlotVerticalClose(myLayout, heightDefault, () =>
        {
            myMask.enabled = false;
            onClaim = false;
            ReloadData();
            AnimOpen();
        }).SetDelay(.25f));

        mySequence.SetDelay(.45f);
        mySequence.Play();
        slotData = AchievementManager.Instance.GetAchievementDataConfig(slotData.achievementType);
    }

    public override void ReloadData()
    {
        InitData(slotData);
    }

    public override void OnChoose()
    {
        if (actionChooseCallBack != null)
            actionChooseCallBack(this);
    }
}
