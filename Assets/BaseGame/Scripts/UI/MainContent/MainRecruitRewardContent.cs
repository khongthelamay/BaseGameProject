using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MainRecruitRewardContent : MainContent<RecruitReward>
{
    [SerializeField] Transform pointStart;
    [SerializeField] Transform pointEnd;
    [SerializeField] Transform pointLoose;

    UnityAction<SlotRecruitReward> action;

    [ShowInInspector] List<ILooseRewardFunction> listLooseFunction = new();

    public void SetActionCallbackMoveDone(UnityAction<SlotRecruitReward> action) { this.action = action; }

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
                if (action != null)
                    (slots[i] as SlotRecruitRewardRun).SetActionCallBackMoveDone(action);
            }
            slots[i].gameObject.SetActive(false);
        }
    }

    public void SlotRewardRunNow() {
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].gameObject.SetActive(true);
            (slots[i] as SlotRecruitRewardRun).RunNow(i == slots.Count - 1);
        }
    }

    public void ShowReward(int slotIndex)
    {
        if (mySequence != null)
            mySequence.Kill();
        mySequence = DOTween.Sequence();
        mySequence.Append(DOVirtual.DelayedCall(.15f, ()=> {
            slots[slotIndex].gameObject.SetActive(true);
            slots[slotIndex].AnimOpen();
        }));
    }
}
