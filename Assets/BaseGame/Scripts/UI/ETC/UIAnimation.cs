using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TW.UGUI.Core.Modals;
using UnityEngine;
using UnityEngine.Events;

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
}
