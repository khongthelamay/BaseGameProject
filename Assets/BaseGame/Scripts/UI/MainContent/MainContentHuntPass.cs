using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MainContentHuntPass : MainContent<HunterPass>
{
    public void SetActionClaimPremium(UnityAction<SlotBase<HunterPass>> claimPremieum)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            (slots[i] as SlotHuntPass).SetActionClaimPremieum(claimPremieum);
        }
    }
}
