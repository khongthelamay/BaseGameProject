using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainContent<Data> : MonoBehaviour
{
    public Transform trsContentParent;
    public SlotBase<Data> slotPref;
    public SlotBase<Data> slotTemp;
    public List<SlotBase<Data>> slotBases = new();
    public int totalSlotUsing;
    public virtual void InitData(List<Data> listData) {
        totalSlotUsing = 0;
        for (int i = 0; i < listData.Count; i++)
        {
            slotTemp = GetSlotBase();
            slotTemp.InitData(listData[i]);
            slotTemp.gameObject.SetActive(true);
            totalSlotUsing++;
        }

        for (int i = totalSlotUsing; i < slotBases.Count; i++)
        {
            slotBases[i].gameObject.SetActive(false);
        }
    }

    public SlotBase<Data> GetSlotBase() {
        for (int i = 0; i < slotBases.Count; i++)
        {
            if (!slotBases[i].gameObject.activeSelf)
                return slotBases[i];
        }
        return Instantiate(slotPref, trsContentParent);
    }

    public void ReloadData() {
        for (int i = 0; i < slotBases.Count; i++)
        {
            if (slotBases[i].gameObject.activeSelf)
                slotBases[i].ReloadData();
        }
    }
}
