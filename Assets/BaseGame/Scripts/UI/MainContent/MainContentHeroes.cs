using Core;
using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainContentHeroes : MainContent<Hero>
{

   
    [Header("==== MainContentHeroes ====")]
    public Transform contentNotOwned;
    public RectTransform rectRebuild;

    public override void InitData(List<Hero> datas)
    {
        datas.Sort((a, b) => { return SortDataHeroes(a.HeroStatData.HeroRarity, b.HeroStatData.HeroRarity); });
        for (int i = 0; i < datas.Count; i++)
        {
            slotTemp = GetSlot();
            slotTemp.gameObject.SetActive(true);
           
            slotTemp.SetActionChooseCallBack(ActionSlotCallBack);
            if (!HeroManager.Instance.IsHaveHero(datas[i].HeroStatData.Name))
                slotTemp.transform.SetParent(contentNotOwned);
            else
                slotTemp.transform.SetParent(trsContentParents);
            slotTemp.InitData(datas[i]);
            totalSlotUsing++;
        }
        DeActiveSlotOut(totalSlotUsing);
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectRebuild);
    }

    int SortDataHeroes(Hero.Rarity a, Hero.Rarity b)
    {
        if (a > b)
            return 1;
        if (b > a) return -1;
        return 0;
    }

    public override void ReloadData(Hero data)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].slotData.HeroStatData.Name == data.HeroStatData.Name)
            {
                slots[i].InitData(data);
                if (!HeroManager.Instance.IsHaveHero(data.HeroStatData.Name))
                    slots[i].transform.SetParent(contentNotOwned);
                else
                    slots[i].transform.SetParent(trsContentParents);
            }
        }
    }
}
