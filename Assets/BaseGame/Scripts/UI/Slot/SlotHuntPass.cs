using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using R3;

public class SlotHuntPass : SlotBase<HuntPass>
{
    [Header("---- Slot Hunt Pass ----")]
    public Image imgBGLevel;
    public Button btnPremiumReward;
    public TextMeshProUGUI txtAmountCommond;
    public TextMeshProUGUI txtAmountPremium;
    public TextMeshProUGUI txtLevel;
    public GameObject objCommondClaimed;
    public GameObject objPremiumClaimed;
    public GameObject objCommondLock;
    public GameObject objPremiumLock;
    public GameObject objCanClaim;
    public List<Sprite> sprIcon;

    public UnityAction<SlotBase<HuntPass>> claimPremium;
    public HuntPassData huntPassData = new();

    public override void Awake()
    {
        base.Awake();
        btnPremiumReward.onClick.AddListener(ClaimPremiumReward);
    }

    public void SetActionClaimPremieum(UnityAction<SlotBase<HuntPass>> action) { claimPremium = action; }

    public override void InitData(HuntPass data)
    {
        base.InitData(data);
        // imgIcon.sprite = data.commond reward type
        // imgIcon.sprite = data.premium reward type
        if (huntPassData.level == 0)
        {
            huntPassData = HuntPassManager.Instance.GetHuntPassDataSave(data.level);
            huntPassData?.isClaimedCommond.ReactiveProperty
            .CombineLatest(huntPassData.isClaimedPremium.ReactiveProperty, (claimC, claimP) => (claimC, claimP))
            .Subscribe(ChangeDataSave);
        }

        txtAmountCommond.text = data.commonRewardAmount.ToString();
        txtAmountPremium.text = data.premiumRewardAmount.ToString();
        txtLevel.text = data.level.ToString();

        HuntPassManager huntPassManager = HuntPassManager.Instance;
        bool isClaimedCommond = huntPassManager.IsClaimedComond(data.level);
        bool isClaimedPremium = huntPassManager.IsClaimedPremium(data.level);
        bool isLocked = huntPassManager.IsLock(data.level);
        bool isPremium = huntPassManager.isPremium;
        int huntLevel = huntPassManager.huntLevel;

        objCommondClaimed.SetActive(isClaimedCommond);
        objPremiumClaimed.SetActive(isClaimedPremium);
        objCommondLock.SetActive(isLocked);
        objCanClaim.SetActive(data.level == huntLevel);
        imgBGLevel.sprite = !isLocked ? sprIcon[1] : sprIcon[0];
        objPremiumLock.SetActive(!isPremium || isLocked);
    }

    void ChangeDataSave((bool claimC, bool claimP) Value) {
        Debug.Log("Claimed: " + Value.claimC + " " + Value.claimP);
        InitData(slotData);
    }

    public override void OnChoose()
    {
        base.OnChoose();
    }

    void ClaimPremiumReward() {
        claimPremium?.Invoke(this);
    }

}
