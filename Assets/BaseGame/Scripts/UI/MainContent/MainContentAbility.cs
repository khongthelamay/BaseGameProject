using Core;
using System.Collections.Generic;
using UnityEngine;

public class MainContentAbility : MainContent<Ability>
{
    public override void InitData(List<Ability> datas)
    {
        for (int i = 0; i < slots.Count; i++)
            slots[i].gameObject.SetActive(false);
        base.InitData(datas);
    }
}
