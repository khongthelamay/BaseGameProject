using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TW.Reactive.CustomComponent;
using R3;
using TW.Utility.CustomType;

public class QuestSlot : SlotBase<QuestDataConfig>
{
    [Header("======= QuestSlot =======")]
    [SerializeField] RectMask2D myMask;
    [SerializeField] TextMeshProUGUI txtDes;
    [SerializeField] TextMeshProUGUI txtProgress;
    [SerializeField] TextMeshProUGUI txtReward;
    [SerializeField] Image imgBG;
    [SerializeField] ProgressBar progressBar;
    [SerializeField] LayoutElement myLayout;

    [SerializeField] GameObject objProgressDone;
    [SerializeField] GameObject objNotice;

    [SerializeField] Transform trsProgressDone;
    [SerializeField] Transform trsIconProgressDone;
    [SerializeField] Vector3 vectorRotate;

    ReactiveValue<QuestSave> questSave = new(null);

    float heightDefault;
    Sequence mySequence;

    public override void Awake()
    {
        base.Awake();
        heightDefault = myLayout.preferredHeight;
    }

    public override void InitData(QuestDataConfig data)
    {
        base.InitData(data);
        questSave = QuestManager.Instance.GetQuestSaveData(slotData.questID);
        
        txtReward.text = data.starReward.ToString();
        txtDes.text = string.Format(data.questDes, data.questRequire);
        txtProgress.text = $"{(BigNumber)questSave.Value.progress.Value} / {(BigNumber)data.questRequire}";
        progressBar.ChangeProgress((float)questSave.Value.progress.Value / (float)data.questRequire);
        btnChoose.interactable = QuestManager.Instance.IsCanClaim(questSave.Value.id.Value);
        if (animOnSlot != null)
            animOnSlot.enabled = !btnChoose.interactable;
        objNotice.SetActive(btnChoose.interactable);
        if (questSave.Value.claimed.Value) ActionCallBackClaimed();
    }

    public override void AnimOpen() {
        if (mySequence != null) mySequence.Kill();
        mySequence = DOTween.Sequence();
        trsContent.localScale = Vector3.one;
        myMask.enabled = true;
        mySequence.Append(UIAnimation.AnimSlotVerticalOpen(myLayout, heightDefault,()=> { myMask.enabled = false; }));
    }

    public override void AnimDone()
    {
        if (mySequence != null) mySequence.Kill();
        QuestManager.Instance.ClaimQuest(slotData.questID);
        objProgressDone.SetActive(true);
        objNotice.SetActive(false);
        myMask.enabled = true;
        mySequence = DOTween.Sequence();
        mySequence.Append(trsProgressDone.DOScale(Vector3.one, .15f).From(2f));
        mySequence.Append(trsIconProgressDone.DORotate(Vector3.zero, .15f).From(vectorRotate));
        mySequence.Append(UIAnimation.AnimSlotVerticalClose(myLayout, heightDefault, () =>
        {
            myMask.enabled = false;
            ActionCallBackClaimed();
        }).SetDelay(.25f));
        mySequence.Play();
    }

    void ActionCallBackClaimed() {
        objProgressDone.SetActive(true);
        myLayout.preferredHeight = heightDefault;
        transform.SetAsLastSibling();
    }

    public void ClearAnim() {
        if (mySequence != null)
            mySequence.Kill();
    }

    public override void ReloadData()
    {
        InitData(slotData);
    }

    public bool IsClaimed() { return objProgressDone.activeSelf; }

    public override void OnChoose()
    {
        if (actionChooseCallBack != null)
            actionChooseCallBack(this);
    }
}
