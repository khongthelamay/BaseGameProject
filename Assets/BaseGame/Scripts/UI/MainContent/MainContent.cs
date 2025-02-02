using Core;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MainContent<Data> : MonoBehaviour
{
    public UnityAction<SlotBase<Data>> actionSlotCallBack;
    public Transform trsContentParents;
    public SlotBase<Data> slotPref;
    [HideInInspector] public SlotBase<Data> slotTemp;
    public List<SlotBase<Data>> slots;
    [HideInInspector] public SlotBase<Data> currentSlotOnChoose;

    [HideInInspector] public int totalSlotUsing;

    public Sequence mySequence;

    public virtual void SetActionSlotCallBack(UnityAction<SlotBase<Data>> actionCallBack) { actionSlotCallBack = actionCallBack; }
    
    public virtual void InitData(List<Data> datas) {
        for (int i = 0; i < datas.Count; i++)
        {
            slotTemp = GetSlot();
            slotTemp.gameObject.SetActive(true);
            slotTemp.InitData(datas[i]);
            slotTemp.SetActionChooseCallBack(ActionSlotCallBack);
            totalSlotUsing++;
        }
        DeActiveSlotOut(totalSlotUsing);
    }

    public virtual void AddSlot(Data data) {
        slotTemp = GetSlot();
        slotTemp.gameObject.SetActive(true);
        slotTemp.InitData(data);
        slotTemp.SetActionChooseCallBack(ActionSlotCallBack);
    }

    public virtual void DeActiveSlotOut(int totalSlotusing) {
        for (int i = totalSlotusing; i < slots.Count; i++) {
            if (slots[i].gameObject.activeSelf)
                slots[i].gameObject.SetActive(false);
        }
    }

    public virtual SlotBase<Data> GetSlot() { 
        for (int i = 0;i < slots.Count;i++) {
            if (!slots[i].gameObject.activeSelf)
                return slots[i];
        }
        slots.Add(Instantiate(slotPref, trsContentParents));
        return slots[slots.Count - 1];
    }

    public virtual void ReloadData(int id)
    {
        
    }

    public virtual void ReloadData(Data data)
    {

    }

    public virtual void ReloadData() { }

    public virtual void ActionSlotCallBack(SlotBase<Data> slotBase) {
        currentSlotOnChoose = slotBase;
        if (actionSlotCallBack != null)
            actionSlotCallBack(currentSlotOnChoose);
    }

    public virtual void SortSlot() { }
    public virtual void AnimOpen() {
        
    }
    public virtual void CleanAnimation() {
        if (mySequence != null)
            mySequence.Kill();
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].CleanAnimation();
        }
    }
}
