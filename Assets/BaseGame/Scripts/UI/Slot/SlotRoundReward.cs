using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SlotRoundReward : SlotBase<RoundRewardConfig>
{
    [Header("---- Slot Round Reward ----")]
    public TextMeshProUGUI txtLevel;
    public override void InitData(RoundRewardConfig data)
    {
        base.InitData(data);
        txtLevel.text = data.level.ToString();
    }
}
