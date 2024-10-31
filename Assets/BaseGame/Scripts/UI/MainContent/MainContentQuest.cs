using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainContentQuest : MainContent<QuestDataConfig>
{
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
            if ((slots[i] as SlotQuest).IsClaimed())
                slots[i].transform.SetAsLastSibling();
            if ((slots[i] as SlotQuest).IsCanClaim())
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
