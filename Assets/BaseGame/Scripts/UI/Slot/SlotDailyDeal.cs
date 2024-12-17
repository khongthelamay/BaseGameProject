using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotDailyDeal : SlotBase<DailyDeal>
{
    public ProgressBar progressBar;
    public override void CleanAnimation()
    {
        base.CleanAnimation();
        progressBar.ClearAnimation();   
    }
}
