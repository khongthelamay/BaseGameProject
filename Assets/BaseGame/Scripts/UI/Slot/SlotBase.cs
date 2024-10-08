using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SlotBase<Data> : MonoBehaviour
{
    public Data slotData;
    public Image imgIcon;
    public Button btnChoose;
    public Transform trsContent;
    public AnimOnSlot animOnSlot;
    public Sequence sequence;

    public UnityAction<SlotBase<Data>> actionChooseCallBack;

    public virtual void SetActionChooseCallBack(UnityAction<SlotBase<Data>> actionCallBack) { actionChooseCallBack = actionCallBack; }

    public virtual void Awake() {
        btnChoose.onClick.AddListener(OnChoose);
    }

    public virtual void InitData(Data data) {
        slotData = data;
    }

    public virtual void ReloadData() { 
    
    }

    public virtual void OnChoose() {
        if (animOnSlot == null)
        {
            if (sequence != null)
                sequence.Kill();
            sequence = UIAnimation.BasicButton(trsContent);
        }

        if (actionChooseCallBack != null)
            actionChooseCallBack(this);
    }

    public virtual void AnimOpen() { }
    public virtual void AnimDone() { }

    public virtual void CleanAnimation()
    {
        if (sequence != null)
            sequence.Kill();

        if (animOnSlot != null)
            animOnSlot.CleanAnimation();
    }
}
