using Core;
using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotBuyHero : SlotBase<Hero>
{
    [Button]
    public override void AnimOpen()
    {
        base.AnimOpen();
        if (mySequence != null)
            mySequence.Kill();
        mySequence = DOTween.Sequence();
        mySequence.Append(UIAnimation.AnimSlotDrop(trsContent));
        mySequence.Play();
    }
}
