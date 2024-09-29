using System;
using System.Collections;
using System.Collections.Generic;
using TW.Reactive.CustomComponent;
using UnityEngine;
using UnityEngine.UI;

public class MainContentArtifact : MainContent<ArtifactDataConfig>
{
    [Header(" ======= ARTIFACT MAIN CONTENT ======= ")]
    public Transform trsDeactiveContentParent;
    public RectTransform rectRebuild;
    public override void InitData(List<ArtifactDataConfig> listData)
    {
        totalSlotUsing = 0;
        for (int i = 0; i < listData.Count; i++)
        {
            slotTemp = GetSlot();
            slotTemp.InitData(listData[i]);
            slotTemp.SetActionChooseCallBack(ActionSlotCallBack);

            if (InGameDataManager.Instance.InGameData.ArtifactData.IsHaveThatArtiFact(listData[i].artifactType))
                slotTemp.transform.SetParent(trsContentParents);
            else 
                slotTemp.transform.SetParent(trsDeactiveContentParent);

            slotTemp.gameObject.SetActive(true);
            totalSlotUsing++;
        }

        for (int i = totalSlotUsing; i < slots.Count; i++)
        {
            slots[i].gameObject.SetActive(false);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectRebuild);
    }

    public override void ReloadData(int artifactID)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].slotData.artifactType == (ArtifactType)artifactID)
            {
                slots[i].ReloadData();
                return;
            }
        }
    }
}
