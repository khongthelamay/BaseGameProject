using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainRecruitRewardContent : MainContent<RecruitReward>
{
    [Button]
    public override void InitData(List<RecruitReward> datas)
    {
        base.InitData(datas);
    }

    public void SlotRewardRunNow() {
        for (int i = 0; i < slots.Count; i++)
        {
            (slots[i] as SlotRecruitRewardRun).RunNow();
        }
    }
}
