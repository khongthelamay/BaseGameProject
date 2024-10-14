using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainContentAbility : MainContent<Ability>
{
    public override void InitData(List<Ability> datas)
    {
        base.InitData(datas);
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].slotData.AbilityTrigger == Ability.Trigger.NormalAttack)
                slots[i].gameObject.SetActive(false);
        }
    }
}
