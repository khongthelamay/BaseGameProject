using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotRecruitRewardRun : SlotRecruitReward
{
    [Header("---- Slot Recruit Reward Run ----")]
    public Transform pointStart;
    public Transform pointEnd;
    public float timeMove;
    [ShowInInspector]public ILooseRewardFunction looseRewardFuction;
    
    public override void InitData(RecruitReward data)
    {
        base.InitData(data);
        transform.localPosition = Vector3.zero;
        looseRewardFuction = new LooseRewardByJump();
    }

    [Button]
    public void RunNow()
    {
        if (mySequence != null)
            mySequence.Kill();
        mySequence = DOTween.Sequence();
        mySequence.Append(transform.DOMove(pointEnd.position, timeMove).From(pointStart.position).SetEase(Ease.Linear)).OnComplete(()=> {
            if (isCollect)
                ActionCollect();
            else
                LooseRewardFunction();
        }).SetDelay(.25f * transform.GetSiblingIndex());
    }

    void ActionCollect() { }

    public void LooseRewardFunction() {
        looseRewardFuction.LooseRewardFunction();
    }
}

public interface ILooseRewardFunction {
    void LooseRewardFunction();
}

class LooseRewardByJump : ILooseRewardFunction
{
    public Transform trsJump;
    public void LooseRewardFunction()
    {

    }
}

class LooseRewardByAlien : ILooseRewardFunction
{
    public Transform trsAlien;
    public void LooseRewardFunction()
    {

    }
}
