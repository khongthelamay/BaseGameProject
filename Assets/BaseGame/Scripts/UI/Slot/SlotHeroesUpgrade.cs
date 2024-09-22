using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotHeroesUpgrade : SlotBase<Hero>
{
    public override void InitData(Hero data)
    {
        base.InitData(data);
        imgIcon.sprite = data.SpriteGraphic.sprite;
    }
}
