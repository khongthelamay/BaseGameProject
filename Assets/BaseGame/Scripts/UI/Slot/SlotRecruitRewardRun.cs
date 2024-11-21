using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

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
        slotData = data;
        imgIcon.sprite = data.heroData.HeroSprite;
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
    public void RunNow(bool isLastMove)
    {
        if (mySequence != null)
            mySequence.Kill();
        mySequence = DOTween.Sequence();
        mySequence.Append(transform.DOMove(pointEnd.position, timeMove).From(pointStart.position).SetEase(Ease.Linear)).OnComplete(()=> {
            ActionCollect();
            if (isLastMove)
                ScreensRecruitContext.Events.LastSlotMoveDone?.Invoke();
        }).SetDelay(.25f * transform.GetSiblingIndex()).SetEase(Ease.Linear);

        if (slotData.isMiss)
            mySequence.OnUpdate(() => {
                if (transform.position.x > pointLoose.position.x)
                {
                    if (isLastMove)
                        ScreensRecruitContext.Events.LastSlotMoveDone?.Invoke();
                    LooseRewardFunction();
                    mySequence.Kill();
                }
            });
    }

    void ActionCollect() {
        callBackMoveDone(this);
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
