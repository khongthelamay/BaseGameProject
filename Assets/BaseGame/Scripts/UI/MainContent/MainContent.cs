using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MainContent<Data> : MonoBehaviour
{
    public UnityAction<SlotBase<Data>> actionSlotCallBack;
    public Transform trsContentParents;
    public SlotBase<Data> slotPref;
    public SlotBase<Data> slotTemp;
    public List<SlotBase<Data>> slots;
    public virtual void SetActionSlotCallBack(UnityAction<SlotBase<Data>> actionCallBack) { actionSlotCallBack = actionCallBack; }
    int totalSlotUsing;
    public virtual void InitData(List<Data> datas) {
        for (int i = 0; i < datas.Count; i++)
        {
            slotTemp = GetSlot();
            slotTemp.gameObject.SetActive(true);
            slotTemp.InitData(datas[i]);
            totalSlotUsing++;
        }
    }

    public virtual void DeActiveSlotOut() {
        for (int i = totalSlotUsing; i < slots.Count; i++) {
            if (slots[i].gameObject.activeSelf)
                slots[i].gameObject.SetActive(false);
        }
    }

    public virtual SlotBase<Data> GetSlot() { 
        for (int i = 0;i < slots.Count;i++) {
            if (!slots[i].gameObject.activeSelf)
                return slots[i];
        }
        return Instantiate(slotPref, trsContentParents);
    }
}
