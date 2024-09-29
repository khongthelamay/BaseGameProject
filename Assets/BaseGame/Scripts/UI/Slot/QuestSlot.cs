using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestSlot : SlotBase<QuestDataConfig>
{
    [Header("======= QuestSlot =======")]
    [SerializeField] TextMeshProUGUI txtDes;
    [SerializeField] TextMeshProUGUI txtProgress;
    [SerializeField] TextMeshProUGUI txtReward;
    [SerializeField] Image imgRewardIcon;
    [SerializeField] ProgressBar progressBar;
    [SerializeField] LayoutElement myLayout;
    float heighDefault;
    Sequence mySequence;

    public override void Awake()
    {
        base.Awake();
        heighDefault = myLayout.preferredHeight;
    }

    public override void InitData(QuestDataConfig data)
    {
        AnimOpen();
        base.InitData(data);
        txtReward.text = data.starReward.ToString();
        txtDes.text = string.Format(data.questDes, data.questRequire);
        txtProgress.text = $"0/{data.questRequire}";
        progressBar.ChangeProgress(0);
    }

    void AnimOpen() {
        if (mySequence != null) mySequence.Kill();
        mySequence = DOTween.Sequence();
        trsContent.localScale = Vector3.one;
        mySequence.Append(UIAnimation.AnimSlotVerticalOpen(myLayout, heighDefault));
    }

    public void AnimDone()
    {
        if (mySequence != null) mySequence.Kill();
        mySequence = DOTween.Sequence();
        mySequence.Append(UIAnimation.AnimSlotVerticalClose(myLayout, heighDefault, () => {
            gameObject.SetActive(false);
        }));
    }
}
