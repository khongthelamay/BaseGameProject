using Sirenix.OdinInspector;
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
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].gameObject.SetActive(false);
        }
        base.InitData(datas);
        for (int i = 0; i < slots.Count; i++)
        {
            if (pointStart != null) {
                (slots[i] as SlotRecruitRewardRun).SetPoint(pointStart, pointEnd, pointLoose);
                slots[i].gameObject.SetActive(false);
            }
           
        }
    }

    public void SlotRewardRunNow() {
        for (int i = 0; i < slots.Count; i++)
        { 
            slots[i].gameObject.SetActive(true);
            (slots[i] as SlotRecruitRewardRun).RunNow();
        }
    }
}
