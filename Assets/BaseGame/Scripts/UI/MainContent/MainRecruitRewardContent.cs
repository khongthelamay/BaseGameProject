using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainRecruitRewardContent : MainContent<RecruitReward>
{
    [SerializeField] Transform pointStart;
    [SerializeField] Transform pointEnd;
    [SerializeField] Transform pointLoose;
    [ShowInInspector] List<ILooseRewardFunction> listLooseFunction = new();
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
