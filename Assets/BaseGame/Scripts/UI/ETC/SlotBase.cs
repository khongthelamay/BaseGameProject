using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class SlotBase<Data> : MonoBehaviour
{
    public Data slotData;
    public Image imgIcon;
    public Button btnChoose;
    public UnityAction<SlotBase<Data>> actionChooseCallBack;

    public virtual void Awake() {
        btnChoose.onClick.AddListener(OnChoose);
    }

    public virtual void SetActionChooseCallback(UnityAction<SlotBase<Data>> actionCallback) { actionChooseCallBack = actionCallback; }
    
    public virtual void InitData(Data data) {
        slotData = data;
    }

    public virtual void OnChoose() {
        if (actionChooseCallBack != null)
            actionChooseCallBack(this);
    }

    public virtual void ReloadData()
    {
        
    }
}
