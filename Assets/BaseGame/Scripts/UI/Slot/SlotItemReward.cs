using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SlotItemReward : SlotBase<RecruitReward>
{
    [Header("---- Slot Item Reward ----")]
    public TextMeshProUGUI txtAmount;

    public override void InitData(RecruitReward data)
    {
        base.InitData(data);
        //imgIcon.sprite = data.resource.type;
        txtAmount.text = data.amount.ToString();
    }
}
