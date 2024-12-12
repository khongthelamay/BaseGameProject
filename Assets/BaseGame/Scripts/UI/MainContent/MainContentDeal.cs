using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainContentDeal : MainContent<ShopPackData>
{
    [Title("Main Content Deal")]

    public List<PackID> packIDs = new();
  
    public bool isInitSlot;
    List<ShopPackData> shopPackDatas = new();

    private void Start()
    {
        GetShopPackDatas();
    }

    void GetShopPackDatas() {
        for (int i = 0; i < packIDs.Count; i++)
        {
            ShopPackData shopPackData = new();
            shopPackData = ShopGlobalConfig.Instance.GetShopPackData(packIDs[i]);
            shopPackDatas.Add(shopPackData);
        }
        InitData(shopPackDatas);
    }

    public override void InitData(List<ShopPackData> datas)
    {
        if (isInitSlot)
        {
            base.InitData(datas);
        }
        else {
            for (int i = 0; i < slots.Count; i++)
            {
                slots[i].InitData(datas[i]);
            }
        }
    }
}
