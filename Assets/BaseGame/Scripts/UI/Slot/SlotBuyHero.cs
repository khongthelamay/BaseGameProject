using Core;
using DG.Tweening;
using Sirenix.OdinInspector;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotBuyHero : SlotBase<Hero>
{
    [Header("===SlotBuyHero===")]
    public Image imgBG;
    public Animator animator;

    public override void AnimOpen()
    {
        base.AnimOpen();
        if (mySequence != null)
        {
            mySequence.Kill();
            trsContent.transform.localScale = Vector3.one;
        }
        mySequence = DOTween.Sequence();
        mySequence.Join(UIAnimation.AnimSlotDrop(trsContent, transform.GetSiblingIndex()));
        mySequence.Play();
    }

    public override void InitData(Hero data)
    {
        base.InitData(data);
        animator.runtimeAnimatorController = data.HeroStatData.ImageAnimatorController;
        imgBG.sprite = SpriteGlobalConfig.Instance.GetFrameSprite(data.HeroStatData.HeroRarity);
    }
}
