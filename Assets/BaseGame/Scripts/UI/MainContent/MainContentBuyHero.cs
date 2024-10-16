using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainContentBuyHero : MainContent<Hero>
{
    public override void InitData(List<Hero> datas)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].InitData(datas[i]);
            slots[i].AnimOpen();
        }
    }
}
