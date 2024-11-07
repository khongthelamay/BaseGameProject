using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainContentHeroReward : MainContent<RecruitReward>
{
    public override void InitData(List<RecruitReward> datas)
    {
        base.InitData(datas);
        //for (int i = 0; i < slots.Count; i++) slots[i].gameObject.SetActive(false);
    }

    public override void AnimOpen()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].AnimOpen();
        }
    }
}
