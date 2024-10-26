using Core;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainContentHeroes : MainContent<Hero>
{
    [Header("==== MainContentHeroes ====")]
    public Transform contentNotOwned;
    public RectTransform rectRebuild;
    bool isLevel;
    bool isTier;

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
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectRebuild);
    }

    public void FilterLevel()
    {
        isLevel = !isLevel;
        int compare = 0;
        if (isLevel)
        {
            slots.Sort((a, b) =>
            {
                compare = CompareLevel(
                            (a as SlotHeroesUpgrade).heroSave.level, 
                            (b as SlotHeroesUpgrade).heroSave.level
                        );
                if (compare != 0)
                    return compare;
                else
                    return
                    CompareTier(
                        (int)a.slotData.HeroStatData.HeroRarity, 
                        (int)b.slotData.HeroStatData.HeroRarity
                    );
            });
        }
        else {
            slots.Sort((a, b) =>
            {
                compare = CompareLevel(
                    (b as SlotHeroesUpgrade).heroSave.level, 
                    (a as SlotHeroesUpgrade).heroSave.level
                );

                if (compare != 0)
                    return compare;
                else
                    return
                    CompareTier(
                        (int)b.slotData.HeroStatData.HeroRarity,
                        (int)a.slotData.HeroStatData.HeroRarity
                    );
            });
        }
       

        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].transform.SetSiblingIndex(i);
        }
    }

    int CompareLevel(int aLevel, int bLevel) {
        if (aLevel > bLevel)
            return -1;
        else if (bLevel > aLevel)
            return 1;
        else return 0;
    }

    public void FilterTier()
    {
        isTier = !isTier;
        int compare;
        if (isTier)
        {
            slots.Sort((a, b) =>
            {
                compare = CompareTier(
                    (int)a.slotData.HeroStatData.HeroRarity, (int)b.slotData.HeroStatData.HeroRarity
                );

                if (compare != 0)
                    return compare;
                else 
                    return
                    CompareLevel(
                        (a as SlotHeroesUpgrade).heroSave.level,
                        (b as SlotHeroesUpgrade).heroSave.level
                    );
            });
        }
        else
        {
            slots.Sort((a, b) =>
            {
                compare = CompareTier(
                     (int)b.slotData.HeroStatData.HeroRarity, (int)a.slotData.HeroStatData.HeroRarity
                 );

                if (compare != 0)
                    return compare;
                else
                    return
                    CompareLevel(
                        (b as SlotHeroesUpgrade).heroSave.level,
                        (a as SlotHeroesUpgrade).heroSave.level
                    );
            });
        }

        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].transform.SetSiblingIndex(i);
        }
    }

    int CompareTier(int aTier, int bTier)
    {
        if (aTier > bTier)
            return -1;
        else if (bTier > aTier)
            return 1;
        else return 0;
    }

    public void SelectMythic()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].slotData.HeroStatData.HeroRarity != Hero.Rarity.Mythic)
                slots[i].gameObject.SetActive(false);
        }
    }

    public void SelectGuardian()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].slotData.HeroStatData.HeroRarity != Hero.Rarity.Mythic)
                slots[i].gameObject.SetActive(true);
        }
    }
}
