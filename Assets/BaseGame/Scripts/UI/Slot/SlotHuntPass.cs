using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class SlotHuntPass : SlotBase<HuntPass>
{
    [Header("---- Slot Hunt Pass ----")]
    
    public Button btnPremiumReward;
    public TextMeshProUGUI txtAmountCommond;
    public TextMeshProUGUI txtAmountPremium;
    public TextMeshProUGUI txtLevel;
    public GameObject objCommondClaimed;
    public GameObject objPremiumClaimed;
    public GameObject objCommondLock;
    public GameObject objPremiumLock;

    public UnityAction<SlotBase<HuntPass>> claimPremium;

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
        txtAmountCommond.text = data.commonRewardAmount.ToString();
        txtAmountPremium.text = data.premiumRewardAmount.ToString();
        txtLevel.text = data.level.ToString();

        objCommondClaimed.SetActive(HuntPassManager.Instance.IsClaimedComond(data.level));
        objPremiumClaimed.SetActive(HuntPassManager.Instance.IsClaimedPremium(data.level));

        objCommondLock.SetActive(HuntPassManager.Instance.IsLock(data.level));

        if (HuntPassManager.Instance.isPremium)
        {
            if (!objCommondLock.activeSelf)
                objPremiumLock.SetActive(false);
            else
                objPremiumLock.SetActive(true);
        }
        else 
            objPremiumLock.SetActive(true);

        //HunterPassManager.Instance.IsCanClaim(data.level);
    }

    public override void OnChoose()
    {
        base.OnChoose();
    }

    void ClaimPremiumReward() {
        if (claimPremium != null)
            claimPremium(this);
    }

}
