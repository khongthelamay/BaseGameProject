using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainContentAchievement : MainContent<AchievementDataConfig>
{

    public override void ReloadData(int id)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].gameObject.activeSelf)
            {
                if ((int)slots[i].slotData.achievementType == id)
                {
                    slots[i].ReloadData();
                    break;
                }
            }
        }
    }

    public override void SortSlot()
    {
        for (int i = slots.Count - 1; i >= 0; i--)
        {
            if ((slots[i] as AchievementSlot).IsClaimed())
                slots[i].transform.SetAsLastSibling();
            if ((slots[i] as AchievementSlot).IsCanClaim())
                slots[i].transform.SetAsFirstSibling();
        }
    }

    public override void AnimOpen()
    {
        for (int i = slots.Count - 1; i >= 0; i--)
        {
            slots[i].AnimOpen();
        }
    }
}
