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
}

public enum PackageId
{
    None = 0,
    StarterDeal = 1,
    BombaPack = 2,
    ColdyPack = 3,

    HandfulGolds = 4,
    SackGolds = 5,
    PileGolds = 6,

    HandfulStones = 7,
    SackStones = 8,
    PileStones = 9,

    HandfulDiamonds = 10,
    SackDiamonds = 11,
    PileDiamonds = 12
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
    [field: SerializeField] public PackageId PackageId { get; set; }
    [field: SerializeField] public string PackageName { get; set; }
    [field: SerializeField] public Sprite PackIcon { get; set; }
    [field: SerializeField] public PurchaseType PurchaseType { get; set; }
    [field: SerializeField] public Resource PaymentAmount { get; set; }
    [field: SerializeField] public List<Resource> rewards { get; set; }
    [field: SerializeField] public List<int> heroRewards { get; set; }
}