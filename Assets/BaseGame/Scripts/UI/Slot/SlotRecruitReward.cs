using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SlotRecruitReward : SlotBase<RecruitReward>
{
    public TextMeshProUGUI txtAmount;
    public UnityAction<SlotRecruitReward> callBackMoveDone;
    public Animator animator;
    public Image imgBG;
    public Image imgSubBG;

    HeroConfigData heroConfig;

    public void SetActionCallBackMoveDone(UnityAction<SlotRecruitReward> callBackMoveDone) { this.callBackMoveDone = callBackMoveDone; }

    public override void InitData(RecruitReward data)
    {
        base.InitData(data);
        heroConfig = data.heroData;
        if (txtAmount != null)
            txtAmount.text = data.amount.ToString();
        animator.runtimeAnimatorController = heroConfig.ImageAnimatorController;
        imgBG.sprite = SpriteGlobalConfig.Instance.GetFrameSprite(heroConfig.HeroRarity);
        imgSubBG.sprite = imgBG.sprite;
    }

    public override void AnimOpen()
    {
        CleanAnimation();
        UIAnimation.AnimSlotDrop(trsContent, 0);
    }
}
