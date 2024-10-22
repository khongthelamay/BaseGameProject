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
    public SkeletonGraphic iconHero;
    Spine.AnimationState animState;

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
        iconHero.skeletonDataAsset = data.HeroStatData.HeroSkeletonDataAsset;
        iconHero.Initialize(true);
        animState = iconHero.AnimationState;
        animState.SetAnimation(0, "Idle", true);
        imgBG.sprite = SpriteGlobalConfig.Instance.GetFrameSprite(data.HeroStatData.HeroRarity);
    }
}
