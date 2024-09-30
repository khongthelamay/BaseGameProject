using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TW.UGUI.Core.Modals;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public static class UIAnimation
{
    static UIAnimData animData;
    public static Sequence BasicButton(Transform trsAanim, UnityAction actionCallBack = null) { 
        animData = UIAnimGlobalConfig.Instance.GetAnimData(UIAnimType.ButtonBasic);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(trsAanim.DOScale(Vector3.one * .95f, .1f));
        sequence.Append(trsAanim.DOScale(Vector3.one * 1.05f, .1f));
        sequence.Append(trsAanim.DOScale(Vector3.one, .1f));
        if (actionCallBack != null)
            sequence.OnComplete(() => {
                actionCallBack();
            });
        sequence.Play();
        return sequence;
    }

    public static Sequence AnimSlotUp(Transform trsSlot, UnityAction actionCallBack = null)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(trsSlot.DOScale(Vector3.one *1.1f, .15f));
        sequence.Append(trsSlot.DOScale(Vector3.one, .15f));

        if (actionCallBack != null)
            sequence.OnComplete(() => {
                actionCallBack();
            });
        sequence.Play();

        return sequence;
    }

    public static Sequence AnimSlotDown(Transform trsSlot, UnityAction actionCallBack = null)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(trsSlot.DOScale(Vector3.one * .95f, .15f));

        if (actionCallBack != null)
            sequence.OnComplete(() => {
                actionCallBack();
            });
        sequence.Play();

        return sequence;
    }

    public static Sequence ModalOpen(CanvasGroup MainView, Transform trsContent, UnityAction actionCallBack = null) {

        animData = UIAnimGlobalConfig.Instance.GetAnimData(UIAnimType.OpenPopup);

        Sequence sequence = DOTween.Sequence();

        sequence.Append(MainView.DOFade(1, animData.duration).From(0));
        sequence.Append(trsContent.DOScale(Vector3.one, animData.duration).From(0).SetEase(animData.easeCurve));

        if (actionCallBack != null)
            sequence.OnComplete(() => {
                actionCallBack();
            });
        sequence.Play();

        return sequence;

    }

    public static Sequence ModalClose(CanvasGroup MainView, Transform trsContent, UnityAction actionCallBack = null)
    {
        animData = UIAnimGlobalConfig.Instance.GetAnimData(UIAnimType.ClosePopup);

        Sequence sequence = DOTween.Sequence();

        sequence.Append(trsContent.DOScale(Vector3.one, animData.duration).From(0).SetEase(animData.easeCurve));

        sequence.Append(MainView.DOFade(0, animData.duration).From(1));

        if (actionCallBack != null)
            sequence.OnComplete(() => {
                actionCallBack();
            });

        sequence.Play();

        return sequence;
    }

    public static Sequence AnimSlotVerticalOpen(LayoutElement myLayout, float heighDefault, UnityAction actionCallBack = null)
    {
        Sequence sequence = DOTween.Sequence();

        myLayout.preferredHeight = 0;

        sequence.Append(DOVirtual.Float(0, heighDefault, .15f, (value) =>
            {
                myLayout.preferredHeight = value;
            }).SetDelay(.2f * myLayout.transform.GetSiblingIndex())
        );

        if (actionCallBack != null)
            sequence.OnComplete(() => {
                actionCallBack();
            });

        sequence.Play();

        return sequence;
    }

    public static Sequence AnimSlotVerticalClose(LayoutElement myLayout, float heighDefault, UnityAction actionCallBack = null)
    {
        Sequence sequence = DOTween.Sequence();

        myLayout.preferredHeight = heighDefault;

        sequence.Append(DOVirtual.Float(heighDefault, 0, .15f, (value) => {
            myLayout.preferredHeight = value;
        })
        );

        if (actionCallBack != null)
            sequence.OnComplete(() => {
                actionCallBack();
            });

        //sequence.Play();

        return sequence;
    }
}
