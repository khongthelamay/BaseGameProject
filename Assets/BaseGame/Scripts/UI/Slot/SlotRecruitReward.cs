using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class SlotRecruitReward : SlotBase<RecruitReward>
{
    public TextMeshProUGUI txtAmount;
    public UnityAction<SlotRecruitReward> callBackMoveDone;

    public void SetActionCallBackMoveDone(UnityAction<SlotRecruitReward> callBackMoveDone) { this.callBackMoveDone = callBackMoveDone; }

    public override void InitData(RecruitReward data)
    {
        base.InitData(data);
        imgIcon.sprite = data.heroData.HeroSprite;
        if (txtAmount != null)
            txtAmount.text = data.amount.ToString();
    }

    public override void AnimOpen()
    {
        CleanAnimation();
        UIAnimation.BasicButton(trsContent);
    }
}
