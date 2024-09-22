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
        if (actionChooseCallBack != null)
            actionChooseCallBack(this);
    }
}
