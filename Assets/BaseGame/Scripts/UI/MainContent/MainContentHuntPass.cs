using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MainContentHuntPass : MainContent<HuntPass>
{
    [Title("----MainContentHuntPass----")]
    public RectTransform myScrollRect;
    public void SetActionClaimPremium(UnityAction<SlotBase<HuntPass>> claimPremieum)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            (slots[i] as SlotHuntPass).SetActionClaimPremieum(claimPremieum);
        }
    }

    public void ScrollToSlot(SlotBase<HuntPass> slot)
    {
        float heightElement = slot.GetComponent<RectTransform>().rect.height;
        Debug.Log($"{heightElement * (slot.slotData.level-1)}");
        float position = heightElement * (slot.slotData.level - 2);
        position = Mathf.Clamp(position, 0, heightElement * slots.Count);
        myScrollRect.anchoredPosition = new Vector2(0, position);
    }
}
