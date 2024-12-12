using DG.Tweening;
using Sirenix.OdinInspector;
using TW.Reactive.CustomComponent;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SlotStreak : SlotBase<StreakDataConfig>
{
    [Header("---- Slot Streak ----")]
    public GameObject objLight;
    public List<Sprite> sprChest = new();
    public ReactiveValue<StreakSave> streakSave = new();
    public TextMeshProUGUI txtStreak;
    public override void InitData(StreakDataConfig data)
    {
        base.InitData(data);
        streakSave = QuestManager.Instance.GetStreakSave(data.streak);
        btnChoose.interactable = streakSave.Value.canClaim.Value && !streakSave.Value.claimed;
        txtStreak.text = data.streak.ToString();
        if (btnChoose.interactable)
            CanClaimMode();
        else
            OnNormalMode();
    }

    public override void ReloadData()
    {
        InitData(slotData);
    }

    [Button]
    void OnNormalMode() {
        imgIcon.sprite = sprChest[0];
        objLight.SetActive(false);
        if (mySequence != null)
            mySequence.Kill();
    }

    [Button]
    void CanClaimMode() {
        imgIcon.sprite = sprChest[1];
        objLight.SetActive(true);
        if (mySequence != null)
            mySequence.Kill();
        mySequence = DOTween.Sequence();
        mySequence.Append(UIAnimation.SlotZoomLoop(trsContent));
        mySequence.SetLoops(-1, LoopType.Yoyo);
    }

    public override void OnChoose()
    {
        OnNormalMode();
        base.OnChoose();
    }
}
