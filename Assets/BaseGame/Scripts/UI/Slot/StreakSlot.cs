using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TW.Reactive.CustomComponent;
using UnityEngine;

public class StreakSlot : SlotBase<StreakDataConfig>
{
    public ReactiveValue<StreakSave> streakSave = new();

    public override void InitData(StreakDataConfig data)
    {
        base.InitData(data);
        streakSave = QuestManager.Instance.GetStreakSave(data.streak);
        btnChoose.interactable = QuestManager.Instance.CheckStreakDone(data.streak);
        //if (btnChoose.interactable)
        //    CanClaimMode();
        //else 
        //    OnNormalMode();
    }

    public override void ReloadData()
    {
        base.ReloadData();
    }

    [Button]
    void OnNormalMode() {
        if (mySequence != null)
            mySequence.Kill();
    }

    [Button]
    void CanClaimMode() {
        if (mySequence != null)
            mySequence.Kill();
        mySequence = DOTween.Sequence();
        mySequence.Append(UIAnimation.SlotZoomLoop(trsContent));
    }
}
