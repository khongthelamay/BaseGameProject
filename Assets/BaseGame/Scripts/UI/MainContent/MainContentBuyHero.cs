using Core;
using Sirenix.OdinInspector;
using System.Collections.Generic;

public class MainContentBuyHero : MainContent<Hero>
{
    [Button]
    public override void InitData(List<Hero> datas)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].InitData(datas[i]);
            slots[i].AnimOpen();
        }
    }
}
