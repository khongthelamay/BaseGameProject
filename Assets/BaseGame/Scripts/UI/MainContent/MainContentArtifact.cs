using System.Collections.Generic;
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

            if (InGameDataManager.Instance.InGameData.ArtifactData.IsHaveThatArtiFact(listData[i].id))
                slotTemp.transform.SetParent(trsContentParents);
            else 
                slotTemp.transform.SetParent(trsDeactiveContentParent);
            slotTemp.AnimOpen();
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
            if (!slots[i].gameObject.activeSelf || slots[i].slotData == null)
                continue;
            if (slots[i].slotData.id == artifactID)
            {
                slots[i].ReloadData();
                if (InGameDataManager.Instance.InGameData.ArtifactData.IsHaveThatArtiFact(slots[i].slotData.id))
                    slots[i].transform.SetParent(trsContentParents);
                else 
                    slots[i].transform.SetParent(trsDeactiveContentParent);
                return;
            }
        }
    }
}
