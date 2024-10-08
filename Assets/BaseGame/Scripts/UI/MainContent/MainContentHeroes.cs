using Core;
using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainContentHeroes : MainContent<HeroStatData>
{
    public override void InitData(List<HeroStatData> datas)
    {
        datas.Sort((a,b) => { return SortDataHeroes(a.HeroRarity,b.HeroRarity); });
        base.InitData(datas);
    }

    int SortDataHeroes(Hero.Rarity a, Hero.Rarity b) {
        if (a > b)
            return 1;
        if (b>a) return -1;
        return 0;
    }
}
