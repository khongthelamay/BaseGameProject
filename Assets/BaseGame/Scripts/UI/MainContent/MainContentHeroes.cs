using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainContentHeroes : MainContent<HeroStatData>
{
    [Header("==== MainContentHeroes ====")]
    public Transform contentNotOwned;
    public RectTransform rectRebuild;

    public override void InitData(List<HeroStatData> datas)
    {
        for (int i = 0; i < datas.Count; i++)
        {
            slotTemp = GetSlot();
            slotTemp.gameObject.SetActive(true);
           
            slotTemp.SetActionChooseCallBack(ActionSlotCallBack);
            if (!HeroManager.Instance.IsHaveHero(datas[i].Name))
                slotTemp.transform.SetParent(contentNotOwned);
            else
                slotTemp.transform.SetParent(trsContentParents);
            slotTemp.InitData(datas[i]);
            totalSlotUsing++;
        }
        DeActiveSlotOut(totalSlotUsing);
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectRebuild);
    }

    public override void ReloadData(HeroStatData data)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].slotData.Name == data.Name)
            {
                slots[i].InitData(data);
                if (!HeroManager.Instance.IsHaveHero(data.Name))
                    slots[i].transform.SetParent(contentNotOwned);
                else
                    slots[i].transform.SetParent(trsContentParents);
            }
        }
    }
}
