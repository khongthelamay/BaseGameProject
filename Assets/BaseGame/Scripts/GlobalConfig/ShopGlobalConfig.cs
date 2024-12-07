using UnityEngine;
using Sirenix.Utilities;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ShopGlobalConfig", menuName = "GlobalConfigs/ShopGlobalConfig")]
[GlobalConfig("Assets/Resources/GlobalConfig/")]
public class ShopGlobalConfig : GlobalConfig<ShopGlobalConfig>
{
    public List<ShopPackData> shopPackDatas = new();
    public List<ShopPackData> goldPack = new();
    public List<ShopPackData> mysthicStonePack = new();
    public List<ShopPackData> diamondPack = new();

    public ShopPackData GetShopPackData(PackID packID) {
        if ((int)packID < 100)
            return GetShopPack(packID);

        if ((int)packID < 200)
            return GetGoldPack(packID);

        if ((int)packID < 300)
            return GetMysthicStonePack(packID);

        if ((int)packID < 400)
            return GetDiamondsPack(packID);

        return null;
    }

    public ShopPackData GetShopPack(PackID packID) {
        for (int i = 0; i < shopPackDatas.Count; i++)
        {
            if (shopPackDatas[i].packID == packID)
                return shopPackDatas[i];
        }
        return null;
    }

    public ShopPackData GetGoldPack(PackID packID)
    {
        for (int i = 0; i < goldPack.Count; i++)
        {
            if (goldPack[i].packID == packID)
                return goldPack[i];
        }
        return null;
    }

    public ShopPackData GetMysthicStonePack(PackID packID)
    {
        for (int i = 0; i < mysthicStonePack.Count; i++)
        {
            if (mysthicStonePack[i].packID == packID)
                return mysthicStonePack[i];
        }
        return null;
    }

    public ShopPackData GetDiamondsPack(PackID packID)
    {
        for (int i = 0; i < diamondPack.Count; i++)
        {
            if (diamondPack[i].packID == packID)
                return diamondPack[i];
        }
        return null;
    }
}

public enum PackID
{
    None = 0,

    StarterDeal = 1,
    BombaPack = 2,
    ColdyPack = 3,

    HandfulGolds = 100,
    SackGolds = 101,
    PileGolds = 102,

    HandfulStones = 200,
    SackStones = 201,
    PileStones = 202,

    HandfulDiamonds = 300,
    SackDiamonds = 301,
    PileDiamonds = 302
}

public enum PurchaseType
{
    None = 0,
    ResourcePay = 1,
    IAPPay = 2,
}

[System.Serializable]
public class ShopPackData
{
    [field: SerializeField] public PackID packID { get; set; }
    [field: SerializeField] public string PackageName { get; set; }
    [field: SerializeField] public Sprite PackIcon { get; set; }
    [field: SerializeField] public PurchaseType PurchaseType { get; set; }
    [field: SerializeField] public Resource PaymentAmount { get; set; }
    [field: SerializeField] public List<Resource> rewards { get; set; }
    [field: SerializeField] public List<int> heroRewards { get; set; }
}