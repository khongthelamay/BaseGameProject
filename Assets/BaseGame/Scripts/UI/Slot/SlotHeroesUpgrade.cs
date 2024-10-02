using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotHeroesUpgrade : SlotBase<HeroStatData>
{
    public override void InitData(HeroStatData data)
    {
        base.InitData(data);
        imgIcon.sprite = data.HeroSprite;
    }
}
