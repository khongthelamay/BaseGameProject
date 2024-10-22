using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainContentHeroJob : MainContent<Hero.Job>
{
    public override void InitData(List<Hero.Job> datas)
    {
        for (int i = 0; i < slots.Count; i++)
            slots[i].gameObject.SetActive(false);
        base.InitData(datas);
    }
}
