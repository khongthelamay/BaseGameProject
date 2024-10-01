using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainContentQuest : MainContent<QuestDataConfig>
{
    public void ClearAnim()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            (slots[i] as QuestSlot).ClearAnim();
        }
    }

    public override void ReloadData(int id)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].gameObject.activeSelf)
            {
                if (slots[i].slotData.questID == id)
                {
                    slots[i].ReloadData();
                    break;
                }
            }
        }
    }

    public override void SortSlot()
    {
        for (int i = slots.Count - 1; i >= 0 ; i--)
        {
            if ((slots[i] as QuestSlot).IsClaimed())
                slots[i].transform.SetAsLastSibling();
            if ((slots[i] as QuestSlot).IsCanClaim())
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
