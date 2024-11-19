using Core;
using System.Collections.Generic;

public class MainContentAbility : MainContent<Ability>
{
    public override void InitData(List<Ability> datas)
    {
        for (int i = 0; i < slots.Count; i++)
            slots[i].gameObject.SetActive(false);
        base.InitData(datas);
        for (int i = 0; i < slots.Count; i++)
        {
            //if (slots[i].slotData.AbilityTrigger == Ability.Trigger.NormalAttack)
            //    slots[i].gameObject.SetActive(false);
        }
    }
}
