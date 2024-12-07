using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopPack : MonoBehaviour
{
    public PackID packID;
    public ShopPackData shopPackData;
    public TextMeshProUGUI txtPrice;
    public List<ShopDealReward> shopDealRewards = new();

    private void Awake()
    {
        InitData();
    }

    public void InitData() {
        shopPackData = ShopGlobalConfig.Instance.GetShopPackData(packID);
        if (shopPackData == null)
            return;
        if (txtPrice != null)
            txtPrice.text = shopPackData.PaymentAmount.value.Value.ToString();

        for (int i = 0; i < shopPackData.rewards.Count; i++)
        {
            if (i < shopDealRewards.Count)
                shopDealRewards[i].InitData(shopPackData.rewards[i]);
        }

        for (int i = 0; i < shopPackData.heroRewards.Count; i++)
        {
            if (i < shopDealRewards.Count)
                shopDealRewards[i].InitData(shopPackData.heroRewards[i]);
        }
    }
}
