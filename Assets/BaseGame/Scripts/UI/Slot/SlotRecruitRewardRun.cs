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
    public Transform pointLoose;
    public float timeMove;
    [ShowInInspector]public ILooseRewardFunction looseRewardFuction;
    
    public override void InitData(RecruitReward data)
    {
        base.InitData(data);
        transform.localPosition = Vector3.zero;
        looseRewardFuction = new LooseRewardByJump();
        (looseRewardFuction as LooseRewardByJump).trsJump = transform;
    }

    public void SetPoint(Transform pointStart, Transform pointEnd, Transform pointLoose) {
        this.pointEnd = pointEnd;
        this.pointStart = pointStart;
        this.pointLoose = pointLoose;
    }

    [Button]
    public void RunNow()
    {
        if (mySequence != null)
            mySequence.Kill();
        mySequence = DOTween.Sequence();
        mySequence.Append(transform.DOMove(pointEnd.position, timeMove).From(pointStart.position).SetEase(Ease.Linear)).OnComplete(()=> {
            ActionCollect();
        }).SetDelay(.25f * transform.GetSiblingIndex()).SetEase(Ease.Linear);

        if (slotData.isMiss)
            mySequence.OnUpdate(() => {
                if (transform.position.x > pointLoose.position.x)
                {
                    LooseRewardFunction();
                    mySequence.Kill();
                }
            });
    }

    void ActionCollect() {
        gameObject.SetActive(false);
    }

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
        Debug.Log("Loose reward");
        trsJump.DOLocalMoveY(trsJump.localPosition.y + 100f, .25f).OnComplete(()=> {
            trsJump.gameObject.SetActive(false);
        });
    }
}

class LooseRewardByAlien : ILooseRewardFunction
{
    public Transform trsAlien;
    public void LooseRewardFunction()
    {

    }
}
