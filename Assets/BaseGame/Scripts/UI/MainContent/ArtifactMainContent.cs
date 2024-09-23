using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArtifactMainContent : MainContent<ArtifactDataConfig>
{
    public Transform trsDeactiveContentParent;
    public RectTransform rectRebuild;
    public override void InitData(List<ArtifactDataConfig> listData)
    {
        totalSlotUsing = 0;
        for (int i = 0; i < listData.Count; i++)
        {
            slotTemp = GetSlot();
            slotTemp.InitData(listData[i]);

            //have this artfact
            //slotTemp.transform.SetParent(trsContentParent);

            //haven't this artfact
            //slotTemp.transform.SetParent(trsDeactiveContentParent);

            slotTemp.gameObject.SetActive(true);
            totalSlotUsing++;
        }

        for (int i = totalSlotUsing; i < slots.Count; i++)
        {
            slots[i].gameObject.SetActive(false);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectRebuild);
    }

}
